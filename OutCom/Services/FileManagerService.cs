using Microsoft.EntityFrameworkCore;
using OutCom.Data;
using OutCom.Models;
using System.Security.Claims;

namespace OutCom.Services
{
    public interface IFileManagerService
    {
        Task<List<FileItem>> GetUserFilesAsync(string userId, string? path = null);
        Task<List<FileItem>> GetClientFilesAsync(string clientId, string? path = null);
        Task<List<ApplicationUser>> GetClientsAsync();
        Task<FileItem?> CreateFolderAsync(string name, string path, string ownerId, string? clientId = null);
        Task<FileItem?> SaveFileAsync(string fileName, string path, long size, string mimeType, string ownerId, string? clientId = null);
        Task<FileItem?> SaveFileWithContentAsync(string fileName, string path, Stream fileStream, string mimeType, 
            string ownerId, string webRootPath, string? clientId = null, string title = "", DateTime? ExpirationDate = null);
        Task<bool> DeleteFileItemAsync(int fileItemId, string userId);
        Task<bool> MoveFileItemAsync(int fileItemId, string newPath, string userId);
        Task<List<FileItem>> GetSelectedFileItemsAsync(List<int> fileItemIds, string userId);
        Task<bool> DeleteMultipleFileItemsAsync(List<int> fileItemIds, string userId);
        Task<bool> MoveMultipleFileItemsAsync(List<int> fileItemIds, string newBasePath, string userId);
        Task<bool> CanUserAccessFileAsync(int fileItemId, string userId);
        Task<FileItem?> GetFileItemByIdAsync(int fileItemId);
        Task<List<FileItem>> GetFolderTreeAsync(string userId, bool isAdmin = false);
        Task<bool> UpdateFilePropertiesAsync(int fileItemId, string title, DateTime? expirationDate, string userId);
        Task<bool> UpdateMultipleFilePropertiesAsync(List<int> fileItemIds, DateTime? expirationDate, bool removeExistingDates, string userId);
        Task<FileItem?> SaveFileWithPropertiesAsync(string fileName, string title, DateTime? expirationDate, string path, Stream fileStream, string mimeType, string ownerId, string webRootPath, string? clientId = null);
    }

    public class FileManagerService : IFileManagerService
    {
        private readonly ApplicationDbContext _context;

        public FileManagerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FileItem>> GetUserFilesAsync(string userId, string? path = null)
        {
            var query = _context.FileItems
                .Include(f => f.Owner)
                .Where(f => !f.IsDeleted);

            // Si es admin, puede ver todos los archivos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.UserType != UserType.Admin)
            {
                // Si es cliente, solo ve sus archivos asignados
                query = query.Where(f => f.ClientId == userId || f.OwnerId == userId);
            }

            if (!string.IsNullOrEmpty(path))
            {
                query = query.Where(f => f.Path.StartsWith(path));
            }
            else
            {
                // Mostrar archivos en la raíz (sin path padre)
                query = query.Where(f => !f.Path.Contains("/"));
            }

            return await query.OrderBy(f => f.Type).ThenBy(f => f.Name).ToListAsync();
        }

        public async Task<List<FileItem>> GetClientFilesAsync(string clientId, string? path = null)
        {
            var query = _context.FileItems
                .Include(f => f.Owner)
                .Where(f => !f.IsDeleted && f.ClientId == clientId);

            if (!string.IsNullOrEmpty(path))
            {
                query = query.Where(f => f.Path.StartsWith(path));
            }
            else
            {
                // Mostrar archivos en la raíz (sin path padre)
                query = query.Where(f => !f.Path.Contains("/"));
            }

            return await query.OrderBy(f => f.Type).ThenBy(f => f.Name).ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetClientsAsync()
        {
            return await _context.Users
                .Where(u => u.UserType == UserType.Client && u.IsActive)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<FileItem?> CreateFolderAsync(string name, string path, string ownerId, string? clientId = null)
        {
            // Validación: toda carpeta debe tener un cliente asignado
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("Toda carpeta debe estar asignada a un cliente específico.", nameof(clientId));
            }

            // Validar que el cliente existe y está activo
            var client = await _context.Users.FirstOrDefaultAsync(u => u.Id == clientId && u.UserType == UserType.Client && u.IsActive);
            if (client == null)
            {
                throw new ArgumentException("El cliente especificado no existe o no está activo.", nameof(clientId));
            }

            var folder = new FileItem
            {
                Name = name,
                Path = string.IsNullOrEmpty(path) ? name : $"{path}/{name}",
                Type = FileItemType.Folder,
                OwnerId = ownerId,
                ClientId = clientId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            _context.FileItems.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }

        public async Task<FileItem?> SaveFileAsync(string fileName, string path, long size, string mimeType, string ownerId, string? clientId = null)
        {
            // Validación: no permitir archivos en el directorio raíz
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("No se pueden subir archivos directamente en el directorio raíz. Los archivos deben subirse dentro de una carpeta asignada a un cliente.", nameof(path));
            }

            // Validación: verificar que la carpeta de destino existe y tiene cliente asignado
            var parentFolder = await _context.FileItems
                .FirstOrDefaultAsync(f => f.Path == path && f.Type == FileItemType.Folder && !f.IsDeleted);
            
            if (parentFolder == null)
            {
                throw new ArgumentException("La carpeta de destino no existe.", nameof(path));
            }

            if (string.IsNullOrEmpty(parentFolder.ClientId))
            {
                throw new ArgumentException("No se pueden subir archivos a carpetas que no tienen un cliente asignado.", nameof(path));
            }

            // Usar el clientId de la carpeta padre
            clientId = parentFolder.ClientId;

            var file = new FileItem
            {
                Name = fileName,
                Path = string.IsNullOrEmpty(path) ? fileName : $"{path}/{fileName}",
                Type = FileItemType.File,
                Size = size,
                MimeType = mimeType,
                OwnerId = ownerId,
                ClientId = clientId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            _context.FileItems.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<FileItem?> SaveFileWithContentAsync(string fileName, string path, Stream fileStream, 
            string mimeType, string ownerId, string webRootPath, string? clientId = null, 
            string title = "", DateTime? ExpirationDate = null)
        {
            try
            {
                // Validación: no permitir archivos en el directorio raíz
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException("No se pueden subir archivos directamente en el directorio raíz. Los archivos deben subirse dentro de una carpeta asignada a un cliente.", nameof(path));
                }

                // Validación: verificar que la carpeta de destino existe y tiene cliente asignado
                var parentFolder = await _context.FileItems
                    .FirstOrDefaultAsync(f => f.Path == path && f.Type == FileItemType.Folder && !f.IsDeleted);
                
                if (parentFolder == null)
                {
                    throw new ArgumentException("La carpeta de destino no existe.", nameof(path));
                }

                if (string.IsNullOrEmpty(parentFolder.ClientId))
                {
                    throw new ArgumentException("No se pueden subir archivos a carpetas que no tienen un cliente asignado.", nameof(path));
                }

                // Usar el clientId de la carpeta padre
                clientId = parentFolder.ClientId;

                // Crear el directorio base UserFiles si no existe
                var baseDirectory = Path.Combine(webRootPath, "UserFiles");
                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                }

                // Crear subdirectorios si es necesario
                if (!string.IsNullOrEmpty(path))
                {
                    var subDirectory = Path.Combine(baseDirectory, path.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (!Directory.Exists(subDirectory))
                    {
                        Directory.CreateDirectory(subDirectory);
                    }
                }

                // Generar nombre único si el archivo ya existe
                var finalFileName = fileName;
                var filePath = string.IsNullOrEmpty(path) 
                    ? Path.Combine(baseDirectory, finalFileName)
                    : Path.Combine(baseDirectory, path.Replace("/", Path.DirectorySeparatorChar.ToString()), finalFileName);
                
                var counter = 1;
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var extension = Path.GetExtension(fileName);
                
                while (File.Exists(filePath))
                {
                    finalFileName = $"{nameWithoutExtension}({counter}){extension}";
                    filePath = string.IsNullOrEmpty(path) 
                        ? Path.Combine(baseDirectory, finalFileName)
                        : Path.Combine(baseDirectory, path.Replace("/", Path.DirectorySeparatorChar.ToString()), finalFileName);
                    counter++;
                }

                // Guardar el archivo físico
                using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(fileStreamOutput);
                }

                // Guardar metadatos en la base de datos
                var file = new FileItem
                {
                    Name = finalFileName,
                    Path = string.IsNullOrEmpty(path) ? finalFileName : $"{path}/{finalFileName}",
                    Title = title,
                    ExpirationDate = ExpirationDate,
                    Type = FileItemType.File,
                    Size = fileStream.Length,
                    MimeType = mimeType,
                    OwnerId = ownerId,
                    ClientId = clientId,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _context.FileItems.Add(file);
                await _context.SaveChangesAsync();
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteFileItemAsync(int fileItemId, string userId)
        {
            var fileItem = await _context.FileItems.FirstOrDefaultAsync(f => f.Id == fileItemId);
            if (fileItem == null) return false;

            // Verificar permisos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.UserType != UserType.Admin && fileItem.OwnerId != userId)
            {
                return false;
            }

            fileItem.IsDeleted = true;
            fileItem.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveFileItemAsync(int fileItemId, string newPath, string userId)
        {
            var fileItem = await _context.FileItems.FirstOrDefaultAsync(f => f.Id == fileItemId);
            if (fileItem == null) return false;

            // Verificar permisos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.UserType != UserType.Admin && fileItem.OwnerId != userId)
            {
                return false;
            }

            fileItem.Path = newPath;
            fileItem.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FileItem>> GetSelectedFileItemsAsync(List<int> fileItemIds, string userId)
        {
            var query = _context.FileItems
                .Include(f => f.Owner)
                .Where(f => fileItemIds.Contains(f.Id) && !f.IsDeleted);

            // Verificar permisos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.UserType != UserType.Admin)
            {
                query = query.Where(f => f.OwnerId == userId || f.ClientId == userId);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> DeleteMultipleFileItemsAsync(List<int> fileItemIds, string userId)
        {
            var fileItems = await GetSelectedFileItemsAsync(fileItemIds, userId);
            if (!fileItems.Any()) return false;

            foreach (var item in fileItems)
            {
                item.IsDeleted = true;
                item.ModifiedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveMultipleFileItemsAsync(List<int> fileItemIds, string newBasePath, string userId)
        {
            var fileItems = await GetSelectedFileItemsAsync(fileItemIds, userId);
            if (!fileItems.Any()) return false;

            foreach (var item in fileItems)
            {
                // Actualizar el path basado en el nuevo directorio base
                item.Path = string.IsNullOrEmpty(newBasePath) ? item.Name : $"{newBasePath}/{item.Name}";
                item.ModifiedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanUserAccessFileAsync(int fileItemId, string userId)
        {
            var fileItem = await _context.FileItems.FirstOrDefaultAsync(f => f.Id == fileItemId);
            if (fileItem == null) return false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.UserType == UserType.Admin) return true;

            return fileItem.OwnerId == userId || fileItem.ClientId == userId;
        }

        public async Task<FileItem?> GetFileItemByIdAsync(int fileItemId)
        {
            return await _context.FileItems
                .Include(f => f.Owner)
                .FirstOrDefaultAsync(f => f.Id == fileItemId && !f.IsDeleted);
        }

        public async Task<List<FileItem>> GetFolderTreeAsync(string userId, bool isAdmin = false)
        {
            var query = _context.FileItems
                .Where(f => f.Type == FileItemType.Folder && !f.IsDeleted);

            if (!isAdmin)
            {
                query = query.Where(f => f.OwnerId == userId || f.ClientId == userId);
            }

            return await query.OrderBy(f => f.Path).ToListAsync();
        }

        public async Task<bool> UpdateFilePropertiesAsync(int fileItemId, string title, DateTime? expirationDate, string userId)
        {
            var fileItem = await _context.FileItems.FirstOrDefaultAsync(f => f.Id == fileItemId && !f.IsDeleted);
            if (fileItem == null) return false;

            // Verificar permisos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.UserType != UserType.Admin && fileItem.OwnerId != userId)
            {
                return false;
            }

            // Solo actualizar archivos, no carpetas
            if (fileItem.Type != FileItemType.File) return false;

            fileItem.Title = title?.Trim();
            fileItem.ExpirationDate = expirationDate;
            fileItem.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMultipleFilePropertiesAsync(List<int> fileItemIds, DateTime? expirationDate, bool removeExistingDates, string userId)
        {
            var fileItems = await GetSelectedFileItemsAsync(fileItemIds, userId);
            var files = fileItems.Where(f => f.Type == FileItemType.File).ToList();
            
            if (!files.Any()) return false;

            foreach (var file in files)
            {
                if (removeExistingDates)
                {
                    file.ExpirationDate = null;
                }
                else if (expirationDate.HasValue)
                {
                    file.ExpirationDate = expirationDate.Value;
                }
                file.ModifiedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<FileItem?> SaveFileWithPropertiesAsync(string fileName, string title, DateTime? expirationDate, string path, Stream fileStream, string mimeType, string ownerId, string webRootPath, string? clientId = null)
        {
            try
            {
                // Validación: no permitir archivos en el directorio raíz
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException("No se pueden subir archivos directamente en el directorio raíz. Los archivos deben subirse dentro de una carpeta asignada a un cliente.", nameof(path));
                }

                // Validación: verificar que la carpeta de destino existe y tiene cliente asignado
                var parentFolder = await _context.FileItems
                    .FirstOrDefaultAsync(f => f.Path == path && f.Type == FileItemType.Folder && !f.IsDeleted);
                
                if (parentFolder == null)
                {
                    throw new ArgumentException("La carpeta de destino no existe.", nameof(path));
                }

                if (string.IsNullOrEmpty(parentFolder.ClientId))
                {
                    throw new ArgumentException("No se pueden subir archivos a carpetas que no tienen un cliente asignado.", nameof(path));
                }

                // Usar el clientId de la carpeta padre
                clientId = parentFolder.ClientId;

                // Crear el directorio base UserFiles si no existe
                var baseDirectory = Path.Combine(webRootPath, "UserFiles");
                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                }

                // Crear subdirectorios si es necesario
                if (!string.IsNullOrEmpty(path))
                {
                    var subDirectory = Path.Combine(baseDirectory, path.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (!Directory.Exists(subDirectory))
                    {
                        Directory.CreateDirectory(subDirectory);
                    }
                }

                // Generar nombre único si el archivo ya existe
                var finalFileName = fileName;
                var filePath = string.IsNullOrEmpty(path) 
                    ? Path.Combine(baseDirectory, finalFileName)
                    : Path.Combine(baseDirectory, path.Replace("/", Path.DirectorySeparatorChar.ToString()), finalFileName);
                
                var counter = 1;
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var extension = Path.GetExtension(fileName);
                
                while (File.Exists(filePath))
                {
                    finalFileName = $"{nameWithoutExtension}({counter}){extension}";
                    filePath = string.IsNullOrEmpty(path) 
                        ? Path.Combine(baseDirectory, finalFileName)
                        : Path.Combine(baseDirectory, path.Replace("/", Path.DirectorySeparatorChar.ToString()), finalFileName);
                    counter++;
                }

                // Guardar el archivo físico
                using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(fileStreamOutput);
                }

                // Guardar metadatos en la base de datos con propiedades adicionales
                var file = new FileItem
                {
                    Name = finalFileName,
                    Title = title?.Trim(),
                    Path = string.IsNullOrEmpty(path) ? finalFileName : $"{path}/{finalFileName}",
                    Type = FileItemType.File,
                    Size = fileStream.Length,
                    MimeType = mimeType,
                    ExpirationDate = expirationDate,
                    OwnerId = ownerId,
                    ClientId = clientId,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _context.FileItems.Add(file);
                await _context.SaveChangesAsync();
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
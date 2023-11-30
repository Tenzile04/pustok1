using Microsoft.AspNetCore.Routing.Constraints;

namespace Pustokk.Extencions
{
    public static  class Helper
    {
        public static string SaveFile(string rootParh,string folderName,IFormFile file)
        {
            string fileName=file.FileName.Length>64 ? file.FileName.Substring(file.FileName.Length-64,64) : file.FileName;

            fileName = Guid.NewGuid().ToString() + file.FileName;
            string path=Path.Combine(rootParh,folderName,fileName);
            using(FileStream stream=new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName ;
        }
    }
}

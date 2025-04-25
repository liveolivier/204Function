using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using System;
//using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace fnVignette
{
    public class genererVignette
    {
        [FunctionName("genererVignette")]
        public void Run([BlobTrigger("uploads/{name}", Connection = "")] Stream myBlob, string name, ILogger log,
                        [Blob("thumbnails/{name}", FileAccess.Write)] Stream outputBlob)
        {
            try
            {
                myBlob.Position = 0;
                AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

                log.LogInformation($"Traitement de \n Nom:{name} \n Taille: {myBlob.Length} Bytes");

                // Vérifier si le fichier est un JPG ou JPEG

                String[] allowedExtensions = new[] { ".jpg", ".jpeg" };

                if (!allowedExtensions.Contains(Path.GetExtension(name), StringComparer.OrdinalIgnoreCase))
                {
                    log.LogWarning($"Le fichier {name} ne sera pas traité car il n'est pas dans un format autorisé.");
                    return;
                }


                // Charger et traiter l'image
                using (var image = Image.Load(myBlob))
                {
                    image.Mutate(x => x.Resize(320, 320));
                    image.SaveAsJpeg(outputBlob); // Sauvegarde au format JPG
                    log.LogInformation($"Vignette de {name} créée en JPG.");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Erreur lors de la conversion de {name}: {ex.Message}");
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using VectorDraw.Professional.Components;
using Newtonsoft.Json.Linq;

namespace selfHosted
{
    public class CadController : ApiController
    {

        //get
        public dynamic Get()
        {
            string queResponder = "{'tipo' : 'restful'," +
                "'verbos' : 'get y post'," +
                "'versión' : 'testing'," +
                "'webserver' : 'auto hosting: http://10.16.144.7:9095'}";
            JObject respuesta = JObject.Parse(queResponder);
            return respuesta;
        }

        //get con parametro
        public dynamic Get(int id)
        {
            string queResponder = "{'estado' : 'true'}";
            JObject respuesta = JObject.Parse(queResponder);
            return respuesta;
        }

        //post
        public string Post([FromBody] ModeloDatos atajado)
        {
            vdDocumentComponent vdDocumentComponent1 = new vdDocumentComponent();
            vdDocumentComponent1.Document.OnIsValidOpenFormat += new VectorDraw.Professional.vdObjects.vdDocument.IsValidOpenFormatEventHandler(Document_OnIsValidOpenFormat);
            vdDocumentComponent1.Document.OnLoadUnknownFileName += new VectorDraw.Professional.vdObjects.vdDocument.LoadUnknownFileName(Document_OnLoadUnknownFileName);
            vdDocumentComponent1.Document.OnSaveUnknownFileName += new VectorDraw.Professional.vdObjects.vdDocument.SaveUnknownFileName(Document_OnSaveUnknownFileName);
            vdDocumentComponent1.Document.OnGetSaveFileFilterFormat += new VectorDraw.Professional.vdObjects.vdDocument.GetSaveFileFilterFormatEventHandler(Document_OnGetSaveFileFilterFormat);

            //path a utilizar para escribir y leer los archivos temporarios:
            string pathDeLaburo = @"C:\_desa\asp.net\rest\selfHosted\temporal\";

            //guardar el archivo recibido temporariamente en disco
            Random rnd = new Random();
            int nombreFileParteNumerica = rnd.Next();
            string nombreFile = "archivoRecibido" + Convert.ToString(nombreFileParteNumerica); 
            String filenameTemporal = pathDeLaburo + nombreFile + "." + atajado.origen;

            try
            {
                //decodificar el archivo recibido en formato base64 a byte array:
                Byte[] bytes = Convert.FromBase64String(atajado.archivo);
                //escribir en disco el archivo recibido:
                using (FileStream fileStream = new FileStream(filenameTemporal, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    if (bytes.Length > 0)
                    {
                        fileStream.Write(bytes, 0, bytes.Length);
                    }
                }
                //leer el archivo que se guardó en disco:
                bool suc = vdDocumentComponent1.Document.Open(filenameTemporal);
                //convertir el archivo leido (el que se guardó en disco), y guardarlo en disco:
                bool suc2 = vdDocumentComponent1.Document.SaveAs(pathDeLaburo + nombreFile + "." + atajado.destino);

                if (suc2)
                {
                    string nombreFileConvertido = pathDeLaburo + nombreFile + "." + atajado.destino;
                    byte[] bytesConvertidos = File.ReadAllBytes(nombreFileConvertido);
                    string fileConvertido = Convert.ToBase64String(bytesConvertidos);
                    //borrar el archivo recibido:
                    //File.Delete(filenameTemporal);
                    //borrar el archivo convertido:
                    //File.Delete(nombreFileConvertido);
                    return fileConvertido;
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception)
            {
                return "error";
            }
        }

        void Document_OnGetSaveFileFilterFormat(ref string saveFilter)
        {
            string[] strs = saveFilter.Split(new string[] { "||" }, 2, StringSplitOptions.None);
            string fileExtensions = strs[0] + "|";
            string versions = "";
            if (strs.Length > 1) versions = strs[1];
            fileExtensions += "DWG (*.dwg)|*.dwg|";
            fileExtensions += "DXF (*.dxf)|*.dxf|";
            fileExtensions += "DGN (*.dgn)|*.dgn|";
            saveFilter = fileExtensions + "|" + versions;

        }

        void Document_OnSaveUnknownFileName(object sender, string fileName, out bool success)
        {
            VectorDraw.Professional.vdObjects.vdDocument doc = sender as VectorDraw.Professional.vdObjects.vdDocument;
            success = false;
            if (fileName.EndsWith(".dwg", StringComparison.CurrentCultureIgnoreCase))
            {
                success = vdxFcnv.ConversionUtilities.SaveDwg(doc, fileName);
            }
            else if (fileName.EndsWith(".dgn", StringComparison.CurrentCultureIgnoreCase))
            {
                success = vdxFcnv.ConversionUtilities.SaveDwg(doc, fileName);
            }
            else if (fileName.EndsWith(".dxf", StringComparison.CurrentCultureIgnoreCase))
            {
                doc.Document.Version = "DXF2004";
                vdDXF.vdDXFSAVE savedxf = new vdDXF.vdDXFSAVE();
                success = savedxf.SaveDXF(doc, fileName);

            }

        }

        void Document_OnLoadUnknownFileName(object sender, string fileName, out bool success)
        {
            VectorDraw.Professional.vdObjects.vdDocument doc = sender as VectorDraw.Professional.vdObjects.vdDocument;
            success = false;
            if (fileName.EndsWith(".dwg", true, System.Globalization.CultureInfo.InvariantCulture))
            {
                success = vdxFcnv.ConversionUtilities.OpenDwg(doc, fileName);
                return;
            }
            else if (fileName.EndsWith(".dgn", true, System.Globalization.CultureInfo.InvariantCulture))
            {
                success = vdxFcnv.ConversionUtilities.OpenDwg(doc, fileName);
                return;
            }
            else if (!fileName.EndsWith(".dxf", true, System.Globalization.CultureInfo.InvariantCulture)) return;
            {
                vdDXF.vdDXFopen opendxf = new vdDXF.vdDXFopen();
                doc.ClearAll();
                doc.FreezeActions = true;
                success = opendxf.LoadDXF(doc, fileName);
                doc.EnsureDefaults();
                doc.FreezeActions = false;
                return;
            }
        }

        void Document_OnIsValidOpenFormat(object sender, string extension, ref bool success)
        {
            if (extension.EndsWith(".dwg", true, System.Globalization.CultureInfo.InvariantCulture)) { success = true; return; }
            else if (extension.EndsWith(".dgn", true, System.Globalization.CultureInfo.InvariantCulture)) { success = true; return; }
            else if (extension.EndsWith(".dxf", true, System.Globalization.CultureInfo.InvariantCulture)) { success = true; return; }
            success = false;
        }

    }
}

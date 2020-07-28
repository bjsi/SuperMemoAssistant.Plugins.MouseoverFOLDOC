using Anotar.Serilog;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverFOLDOC
{

  public static class Keywords
  {

    public static Dictionary<string, string> KeywordMap => CreateKeywordMap();
    public static MouseoverFOLDOCCfg Config => Svc<MouseoverFOLDOCPlugin>.Plugin.Config;

    private static Dictionary<string, string> CreateKeywordMap()
    {
      // Copied manually to development plugins folder
      // var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
      // TODO: Wouldn't work unless hard coded ?????????????

      var jsonPath = Path.Combine(@"C:\Users\james\SuperMemoAssistant\Plugins\Development\SuperMemoAssistant.Plugins.MouseoverFOLDOC\dictionary\dictionary_entries.json");

      try
      {

        using (StreamReader r = new StreamReader(jsonPath))
        {

          string json = r.ReadToEnd();
          var jObj = json.Deserialize<Dictionary<string, string>>();
          return jObj;

        }
      }
      catch (FileNotFoundException)
      {

        LogTo.Error($"Failed to CreateKeywordMap because {jsonPath} does not exist");
        return null;

      }
      catch (IOException e)
      {

        LogTo.Error($"Exception {e} thrown when attempting to read from {jsonPath}");
        return null;

      }
    }

  }
}

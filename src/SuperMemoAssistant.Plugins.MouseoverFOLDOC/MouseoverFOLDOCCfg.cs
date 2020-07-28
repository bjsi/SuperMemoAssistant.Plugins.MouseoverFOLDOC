using Forge.Forms.Annotations;
using Newtonsoft.Json;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverFOLDOC
{

  [Form(Mode = DefaultFields.None)]
  [Title("Dictionary Settings",
     IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
    "Cancel",
    IsCancel = true)]
  [DialogAction("save",
    "Save",
    IsDefault = true,
    Validates = true)]
  public class MouseoverFOLDOCCfg : CfgBase<MouseoverFOLDOCCfg>, INotifyPropertyChangedEx
  {
    [Title("Mouseover Free Online Dictionary of Computing")]

    [Heading("By Jamesb | Experimental Learning")]

    [Heading("Features:")]
    [Text(@"- Load dictionary definitions from the Free Online Dictionary of Computing")]

    [Heading("Keyword Scanning Settings")]

    [Heading("Reference Regexes")]
    [Field(Name = "Title Regexes")]
    [MultiLine]
    public string ReferenceTitleRegexes { get; set; } = ".*Computer Science.*";

    [Field(Name = "Author Regexes")]
    [MultiLine]
    public string ReferenceAuthorRegexes { get; set; }

    [Field(Name = "Source Regexes")]
    [MultiLine]
    public string ReferenceSourceRegexes { get; set; }

    [Field(Name = "Link Regexes")]
    public string ReferenceLinkRegexes { get; set; }

    [Field(Name = "Concept Regexes")]
    [MultiLine]
    public string ConceptNameRegexes { get; set; } = "Computer Science";

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public override string ToString()
    {
      return "Mouseover Free Online Dictionary of Computing Settings";
    }

    public event PropertyChangedEventHandler PropertyChanged;

  }
}

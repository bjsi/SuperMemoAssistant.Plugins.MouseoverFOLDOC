#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   7/25/2020 4:30:01 AM
// Modified By:  james

#endregion




namespace SuperMemoAssistant.Plugins.MouseoverFOLDOC
{
  using System.Diagnostics.CodeAnalysis;
  using Anotar.Serilog;
  using MouseoverPopup.Interop;
  using SuperMemoAssistant.Services;
  using SuperMemoAssistant.Services.IO.HotKeys;
  using SuperMemoAssistant.Services.Sentry;
  using SuperMemoAssistant.Services.UI.Configuration;

  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  public class MouseoverFOLDOCPlugin : SentrySMAPluginBase<MouseoverFOLDOCPlugin>
  {
    #region Constructors

    /// <inheritdoc />
    public MouseoverFOLDOCPlugin() : base("Enter your Sentry.io api key (strongly recommended)") { }

    #endregion

    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "MouseoverFOLDOC";

    /// <inheritdoc />
    public override bool HasSettings => false;

    public MouseoverFOLDOCCfg Config;

    // Reference Regexes
    private string[] TitleRegexes => Config.ReferenceTitleRegexes?.Replace("\r\n", "\n")?.Split('\n');
    private string[] AuthorRegexes => Config.ReferenceAuthorRegexes?.Replace("\r\n", "\n")?.Split('\n');
    private string[] LinkRegexes => Config.ReferenceLinkRegexes?.Replace("\r\n", "\n")?.Split('\n');
    private string[] SourceRegexes => Config.ReferenceSourceRegexes?.Replace("\r\n", "\n")?.Split('\n');

    // Category Path Regexes
    private string[] CategoryPathRegexes => Config.ConceptNameRegexes?.Replace("\r\n", "\n")?.Split('\n');

    // Regex
    public readonly string DictRegex = @"https://foldoc.org/";

    // Content Service
    private ContentService _contentService => new ContentService();

    #endregion

    #region Methods Impl

    public override void ShowSettings()
    {
      ConfigurationWindow.ShowAndActivate(HotKeyManager.Instance, Config);
    }

    private void LoadConfig()
    {
      Config = Svc.Configuration.Load<MouseoverFOLDOCCfg>() ?? new MouseoverFOLDOCCfg();
    }


    /// <inheritdoc />
    protected override void PluginInit()
    {

      LoadConfig();

      var refs = new ReferenceRegexes(TitleRegexes, AuthorRegexes, LinkRegexes, SourceRegexes);
      var opts = new KeywordScanningOptions(refs, Keywords.KeywordMap, MapType.URL, CategoryPathRegexes);

      if (!this.RegisterProvider(Name, new string[] { DictRegex }, opts, _contentService))
      {
        LogTo.Error("Failed to register provider with MouseoverPopup");
        return;
      }

      LogTo.Error("Successfully registered provider with MouseoverPopup");

    }

    #endregion

    #region Methods

    #endregion
  }
}

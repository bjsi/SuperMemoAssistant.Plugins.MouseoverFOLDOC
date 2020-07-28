using Anotar.Serilog;
using HtmlAgilityPack;
using MouseoverPopup.Interop;
using PluginManager.Interop.Sys;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverFOLDOC
{

  [Serializable]
  public class ContentService : PerpetualMarshalByRefObject, IMouseoverContentProvider
  {

    private string DictRegex = Svc<MouseoverFOLDOCPlugin>.Plugin.DictRegex;

    private readonly HttpClient _httpClient;

    public ContentService()
    {
      _httpClient = new HttpClient();
      _httpClient.DefaultRequestHeaders.Accept.Clear();
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void Dispose()
    {
      _httpClient?.Dispose();
    }

    public RemoteTask<PopupContent> FetchHtml(RemoteCancellationToken ct, string href)
    {
      try
      {

        if (href.IsNullOrEmpty() || !new Regex(DictRegex).Match(href).Success)
          return null;

        return GetDictionaryEntry(ct.Token(), href);

      }
      catch (TaskCanceledException) { }
      catch (Exception ex)
      {
        LogTo.Error($"Failed to FetchHtml for href {href} with exception {ex}");
        throw;
      }

      return null;
    }


    public async Task<PopupContent> GetDictionaryEntry(CancellationToken ct, string url)
    {

      string response = await GetAsync(ct, url);
      return CreatePopupContent(response, url);

    }

    public PopupContent CreatePopupContent(string content, string url) 
    {

      if (content.IsNullOrEmpty() || url.IsNullOrEmpty())
        return null;

      var doc = new HtmlDocument();
      doc.LoadHtml(content);

      doc = doc.ConvRelToAbsLinks("https://foldoc.org");

      var contentNode = doc.DocumentNode.SelectSingleNode("//div[@id='content']");
      if (contentNode.IsNull())
        return null;

      var titleNode = contentNode.SelectSingleNode("//h2");
      if (titleNode.IsNull())
        return null;

      string html = @"
          <html>
            <body>
              <p>{0}</p>
              <small>{1}</small>
            </body>
          </html>";

      html = String.Format(html, contentNode.InnerHtml, Svc<MouseoverFOLDOCPlugin>.Plugin.Name);

      var refs = new References();
      refs.Title = titleNode.InnerText;
      refs.Source = "Free Online Dictionary of Computing";
      refs.Link = url;

      return new PopupContent(refs, html, true, browserQuery: url);

    }

    private async Task<string> GetAsync(CancellationToken ct, string url)
    {
      HttpResponseMessage responseMsg = null;

      try
      {
        responseMsg = await _httpClient.GetAsync(url, ct);

        if (responseMsg.IsSuccessStatusCode)
        {
          return await responseMsg.Content.ReadAsStringAsync();
        }
        else
        {
          return null;
        }
      }
      catch (HttpRequestException)
      {
        if (responseMsg != null && responseMsg.StatusCode == System.Net.HttpStatusCode.NotFound)
          return null;
        else
          throw;
      }
      catch (OperationCanceledException)
      {
        return null;
      }
      finally
      {
        responseMsg?.Dispose();
      }
    }
  }
}

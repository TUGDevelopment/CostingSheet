using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public static class DocumentFormatHelper
{
	static Dictionary<string, object> RichEditFormatRegistry;
	static Dictionary<string, object> SpreadsheetFormatRegistry;

	static DocumentFormatHelper() {
		RichEditFormatRegistry = new Dictionary<string, object>();
		SpreadsheetFormatRegistry = new Dictionary<string, object>();

		RichEditFormatRegistry.Add("doc", DevExpress.XtraRichEdit.DocumentFormat.Doc);
		RichEditFormatRegistry.Add("epub", DevExpress.XtraRichEdit.DocumentFormat.ePub);
		RichEditFormatRegistry.Add("html", DevExpress.XtraRichEdit.DocumentFormat.Html);
		RichEditFormatRegistry.Add("htm", DevExpress.XtraRichEdit.DocumentFormat.Html);
		RichEditFormatRegistry.Add("mht", DevExpress.XtraRichEdit.DocumentFormat.Mht);
		RichEditFormatRegistry.Add("odt", DevExpress.XtraRichEdit.DocumentFormat.OpenDocument);
		RichEditFormatRegistry.Add("docx", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
		RichEditFormatRegistry.Add("txt", DevExpress.XtraRichEdit.DocumentFormat.PlainText);
		RichEditFormatRegistry.Add("rtf", DevExpress.XtraRichEdit.DocumentFormat.Rtf);
		RichEditFormatRegistry.Add("xml", DevExpress.XtraRichEdit.DocumentFormat.WordML);

		SpreadsheetFormatRegistry.Add("csv", DevExpress.Spreadsheet.DocumentFormat.Csv);
		SpreadsheetFormatRegistry.Add("xls", DevExpress.Spreadsheet.DocumentFormat.Xls);
		SpreadsheetFormatRegistry.Add("xlsm", DevExpress.Spreadsheet.DocumentFormat.Xlsm);
		SpreadsheetFormatRegistry.Add("xlsx", DevExpress.Spreadsheet.DocumentFormat.Xlsx);
		SpreadsheetFormatRegistry.Add("xlt", DevExpress.Spreadsheet.DocumentFormat.Xlt);
	}

	public static object GetFormat(string path) {
		var extension = Path.GetExtension(path).Trim('.', ' ').ToLower();
		object format = null;
		if (RichEditFormatRegistry.TryGetValue(extension, out format))
			return format;
		if (SpreadsheetFormatRegistry.TryGetValue(extension, out format))
			return format;
		return null;
	}

}
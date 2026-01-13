using System;
using System.Collections.Generic;

namespace Retailer.Api.Helpers
{
    public static class ExportFileResolver
    {
        private static readonly Dictionary<string, ExportFileInfo> _map =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["pdf"] = new ExportFileInfo("application/pdf", "pdf"),
                ["excel"] = new ExportFileInfo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"),
                ["word"] = new ExportFileInfo("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx")
            };

        public static ExportFileResult Resolve(string? export, string baseFileName)
        {
            if (string.IsNullOrWhiteSpace(baseFileName))
                throw new ArgumentException("Base filename cannot be empty", nameof(baseFileName));

            var key = export?.Trim() ?? "pdf";

            if (!_map.TryGetValue(key, out var info))
                info = _map["pdf"];

            var fileName = $"{baseFileName}_{DateTime.Now:yyyyMMdd}.{info.Extension}";

            return new ExportFileResult(info.ContentType, info.Extension, fileName);
        }

        public static void Register(string key, string contentType, string extension)
        {
            _map[key] = new ExportFileInfo(contentType, extension);
        }
    }

    public sealed class ExportFileInfo
    {
        public string ContentType { get; }
        public string Extension { get; }

        public ExportFileInfo(string contentType, string extension)
        {
            ContentType = contentType;
            Extension = extension;
        }
    }

    public sealed class ExportFileResult
    {
        public string ContentType { get; }
        public string Extension { get; }
        public string FileName { get; }

        public ExportFileResult(string contentType, string extension, string fileName)
        {
            ContentType = contentType;
            Extension = extension;
            FileName = fileName;
        }
    }
}

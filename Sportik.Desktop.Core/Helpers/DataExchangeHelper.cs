using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Common.Export;
using Sportik.Desktop.Core.Common.Import;

namespace Sportik.Desktop.Core.Helpers
{
    public static class DataExchangeHelper
    {
        public static bool TrySerialize(IDataExchangeConfig config, out string serializedConfig)
        {
            try
            {
                serializedConfig = config.Type switch
                {
                    DataExchangeType.Import => SerializeImportConfig(config),
                    DataExchangeType.Export => SerializeExportConfig(config),
                    _ => throw new NotSupportedException($"Data exchange type '{config.Type}' is not supported.")
                };

                return true;
            }
            catch (Exception)
            {
                serializedConfig = null;
                return false;
            }
        }

        public static bool TryDeserialize(string serializedConfig, out IDataExchangeConfig config)
        {
            try
            {
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedConfig);

                if (values == null || !values.TryGetValue("Type", out string typeString) || !Enum.TryParse(typeString, out DataExchangeType type))
                {
                    config = null;
                    return false;
                }

                config = type switch
                {
                    DataExchangeType.Import => DeserializeImportConfig(values),
                    DataExchangeType.Export => DeserializeExportConfig(values),
                    _ => throw new NotSupportedException($"Data exchange type '{type}' is not supported.")
                };

                return true;
            }
            catch (Exception)
            {
                config = null;
                return false;
            }
        }

        private static string SerializeImportConfig(IDataExchangeConfig config)
        {
            if (config is GoogleSheetsImporterConfig googleSheetsImporterConfig)
            {
                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "Type", googleSheetsImporterConfig.Type.ToString() },
                    { "Option", googleSheetsImporterConfig.Option.ToString() },
                    { "SheetUrlOrId", googleSheetsImporterConfig.SheetUrlOrId },
                    { "ExercisesSheetName", googleSheetsImporterConfig.ExercisesSheetName },
                    { "SetsSheetName", googleSheetsImporterConfig.SetsSheetName },
                    { "ValidateDuplicates", googleSheetsImporterConfig.ValidateDuplicates.ToString() }
                };

                return JsonConvert.SerializeObject(values);
            }

            throw new NotSupportedException($"Data exchange config type '{config.GetType().Name}' is not supported for import serialization.");
        }

        private static string SerializeExportConfig(IDataExchangeConfig config)
        {
            if (config is GoogleSheetsExporterConfig googleSheetsExporterConfig)
            {
                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "Type", googleSheetsExporterConfig.Type.ToString() },
                    { "Option", googleSheetsExporterConfig.Option.ToString() },
                    { "SheetUrlOrId", googleSheetsExporterConfig.SheetUrlOrId },
                    { "ExercisesSheetName", googleSheetsExporterConfig.ExercisesSheetName },
                    { "SetsSheetName", googleSheetsExporterConfig.SetsSheetName }
                };

                return JsonConvert.SerializeObject(values);
            }

            throw new NotSupportedException($"Data exchange config type '{config.GetType().Name}' is not supported for export serialization.");
        }

        private static IDataExchangeConfig DeserializeImportConfig(Dictionary<string, string> values)
        {
            if (values.TryGetValue("Option", out string optionString) && Enum.TryParse(optionString, out DataExchangeOption option))
            {
                return option switch
                {
                    DataExchangeOption.GoogleSheets => new GoogleSheetsImporterConfig(
                        values["SheetUrlOrId"],
                        values["ExercisesSheetName"],
                        values["SetsSheetName"],
                        bool.Parse(values["ValidateDuplicates"])),
                    _ => throw new NotSupportedException($"Data exchange option '{option}' is not supported for import deserialization.")
                };
            }

            throw new InvalidOperationException("Invalid data exchange config for import deserialization.");
        }

        private static IDataExchangeConfig DeserializeExportConfig(Dictionary<string, string> values)
        {
            if (values.TryGetValue("Option", out string optionString) && Enum.TryParse(optionString, out DataExchangeOption option))
            {
                return option switch
                {
                    DataExchangeOption.GoogleSheets => new GoogleSheetsExporterConfig(
                        values["SheetUrlOrId"],
                        values["ExercisesSheetName"],
                        values["SetsSheetName"]),
                    _ => throw new NotSupportedException($"Data exchange option '{option}' is not supported for export deserialization.")
                };
            }

            throw new InvalidOperationException("Invalid data exchange config for export deserialization.");
        }
    }
}
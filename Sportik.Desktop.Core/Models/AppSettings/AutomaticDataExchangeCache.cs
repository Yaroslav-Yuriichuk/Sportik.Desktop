using System.Collections.Generic;
using Newtonsoft.Json;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Helpers;

namespace Sportik.Desktop.Core.Models.AppSettings
{
    internal sealed class AutomaticDataExchangeCache
    {
        [JsonProperty]
        private HashSet<string> _configs = new HashSet<string>();

        [JsonIgnore]
        public IReadOnlyList<IDataExchangeConfig> Configs
        {
            get
            {
                List<IDataExchangeConfig> configs = new List<IDataExchangeConfig>();

                foreach (string serializedConfig in _configs)
                {
                    if (DataExchangeHelper.TryDeserialize(serializedConfig, out IDataExchangeConfig config))
                    {
                        configs.Add(config);
                    }
                }

                return configs;
            }
        }

        public bool TryAddConfig(IDataExchangeConfig config)
        {
            return DataExchangeHelper.TrySerialize(config, out string serializedConfig) && _configs.Add(serializedConfig);
        }

        public bool TryRemoveConfig(IDataExchangeConfig config)
        {
            return DataExchangeHelper.TrySerialize(config, out string serializedConfig) && _configs.Remove(serializedConfig);
        }
    }
}
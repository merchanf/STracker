using System;
using Nest;

namespace Mistaker
{
    class ElasticController
    {
        private static Uri _node;
        private static ConnectionSettings _settings;
        private static ElasticClient _elasticClient;

        private static readonly string DefaultIndex = Config.DefaultIndex;
        private static readonly string AvailClosedIndex = Config.AvailClosedIndex;

        private static ElasticClient GetElasticClient()
        {
            if (_node == null || _settings == null || _elasticClient == null)
            {
                _node = new Uri("http://localhost:9200");
                _settings = new ConnectionSettings(_node);
                _settings.DefaultIndex(DefaultIndex);
                _elasticClient = new ElasticClient(_settings);
            }
            return _elasticClient;
        }

        public static void AddElasticEoErrors(EoError[] errors)
        {
            foreach (var error in errors)
            {
                AddElasticEoError(error);
            }
        }

        public static void AddElasticEoAvailClosedErrors(EoAvailClosedError[] errors)
        {
            foreach (var error in errors)
            {
                AddElasticEoAvailClosedError(error);
            }
        }

        private static void AddElasticEoError(EoError error)
        {
            GetElasticClient().Index(error);
        }

        private static void AddElasticEoAvailClosedError(EoAvailClosedError error)
        {
            GetElasticClient().Index(error, i => i.Index(AvailClosedIndex));
        }
    }
}

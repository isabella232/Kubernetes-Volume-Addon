namespace KubernetesVolume.Addon.Clients
{
    using Newtonsoft.Json;
    using RestSharp.Serializers;

    public class JsonSerializer : ISerializer
    {
        public JsonSerializer()
        {
            ContentType = "application/json";
        }

        public string ContentType { get; set; }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}

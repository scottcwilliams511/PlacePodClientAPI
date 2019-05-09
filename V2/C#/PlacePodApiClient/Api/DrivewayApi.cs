using PlacePodApiClient.Lib;

namespace PlacePodApiClient.Api {
    /// <summary>
    /// Contains methods routes under the base route of '/driveways'.
    /// </summary>
    public class DrivewayApi : BaseApi {
        public const string Path = "driveways";

        public DrivewayApi(IHttpAsync httpAsync) : base(httpAsync, Path) { }
    }
}

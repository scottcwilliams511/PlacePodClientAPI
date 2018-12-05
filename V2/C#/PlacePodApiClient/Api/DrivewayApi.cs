using PlacePodApiClient.Lib;

namespace PlacePodApiClient.Api {

    public class DrivewayApi : BaseApi {

        public DrivewayApi(IHttpAsync http) : base (http, "/driveways") { }
    }
}

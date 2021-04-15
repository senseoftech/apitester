namespace Sot.ApiTester.Domain
{
    public class Step
    {
        public Step()
        {
            Request = new Request();
            Response = new Response();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
    }
}

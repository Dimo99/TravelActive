using TravelActive.Data;

namespace TravelActive.Services
{
    public abstract class Service
    {
        protected TravelActiveContext Context;

        protected Service(TravelActiveContext context)
        {
            this.Context = context;
        }
    }
}
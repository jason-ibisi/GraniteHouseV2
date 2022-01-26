using Braintree;

namespace GraniteHouseV2_Utility.BrainTree
{
    public interface IBrainTreeGateway
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}

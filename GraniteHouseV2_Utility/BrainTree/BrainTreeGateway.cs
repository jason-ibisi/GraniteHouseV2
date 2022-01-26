using Braintree;
using Microsoft.Extensions.Options;

namespace GraniteHouseV2_Utility.BrainTree
{
    public class BrainTreeGateway : IBrainTreeGateway
    {
        public BrainTreeSettings _options { get; set; }
        public IBraintreeGateway _braintreeGateway { get; set; }
        
        public BrainTreeGateway(IOptions<BrainTreeSettings> options /*bind BrainTreeSettings to appsettings.json*/)
        {
            _options = options.Value;
        }

        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(_options.Environment, _options.MerchantId, _options.PublicKey, _options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (_braintreeGateway == null)
            {
                _braintreeGateway = CreateGateway();
            }
            return _braintreeGateway;
        }
    }
}

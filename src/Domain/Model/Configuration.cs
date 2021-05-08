// ==================================
// Configuration object model class
// ==================================

namespace CoWinService.Domain.Model
{
    // configuration object model
    public class Configuration
    {
        internal string CoWinEndpoint { get; set; }
        internal string SnsTopicArn { get; set; }
    }
}

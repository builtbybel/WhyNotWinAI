using System.Drawing;
using System.Threading.Tasks;

namespace WhyNotWinAI
{
    public abstract class SystemCheck
    {
        public abstract Task<(string message, Color color)> RunCheck(Logger logger);
    }
}
using System.Threading.Tasks;

namespace Sample.Main.Common
{
    public interface IAsyncInitialization
    {
        Task Initialization { get; }
    }
}

using Unity.Netcode.Components;

namespace Main.Scripts.Utilities
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

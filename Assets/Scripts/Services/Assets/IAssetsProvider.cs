using Services.ServiceLocator;
using UnityEngine;

namespace Services.Assets
{
    public interface IAssetsProvider : IService
    {
        GameObject InstantiateEntity(string path);
    }
}
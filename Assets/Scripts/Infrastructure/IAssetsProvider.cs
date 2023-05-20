using UnityEngine;

namespace Infrastructure
{
    public interface IAssetsProvider : IService
    {
        GameObject InstantiateEntity(string path);
    }
}
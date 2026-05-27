using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HubSpawnManagerTests
{
    private GameObject _managerObject;
    private GameObject _playerObject;
    private GameObject _spawnPointObject;

    [TearDown]
    public void TearDown()
    {
        if (_managerObject != null) Object.Destroy(_managerObject);
        if (_playerObject != null) Object.Destroy(_playerObject);
        if (_spawnPointObject != null) Object.Destroy(_spawnPointObject);
    }

    [UnityTest]
    public IEnumerator Start_ShouldMovePlayerToSpawnPointPosition()
    {
        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";
        _playerObject.transform.position = new Vector3(10f, 10f, 10f);

        _spawnPointObject = new GameObject("SpawnPoint");
        _spawnPointObject.transform.position = new Vector3(1f, 2f, 3f);

        _managerObject = new GameObject("HubSpawnManager");
        HubSpawnManager spawnManager = _managerObject.AddComponent<HubSpawnManager>();
        spawnManager.spawnPoint = _spawnPointObject.transform;

        yield return null;

        Assert.AreEqual(new Vector3(1f, 2f, 3f), _playerObject.transform.position);
    }
}
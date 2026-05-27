using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BossAndVentControllersTests
{
    private GameObject _holder;
    private BossRoomController _bossRoomController;
    private VentController _ventController;
    private GameObject _closedVent;
    private GameObject _openVent;

    [SetUp]
    public void SetUp()
    {
        _holder = new GameObject();
        _bossRoomController = _holder.AddComponent<BossRoomController>();
        _ventController = _holder.AddComponent<VentController>();

        _closedVent = new GameObject("Closed");
        _openVent = new GameObject("Open");

        typeof(VentController).GetField("closedVent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_ventController, _closedVent);
        typeof(VentController).GetField("openVent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_ventController, _openVent);

        _bossRoomController.vent = _ventController;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_holder);
        Object.Destroy(_closedVent);
        Object.Destroy(_openVent);
    }

    [UnityTest]
    public IEnumerator VentController_Start_ShouldSetInitialClosedState()
    {
        _ventController.SendMessage("Start");
        yield return null;

        Assert.IsTrue(_closedVent.activeSelf);
        Assert.IsFalse(_openVent.activeSelf);
    }

    [UnityTest]
    public IEnumerator HandleBossDeath_ShouldOpenVent()
    {
        _ventController.SendMessage("Start");
        yield return null;

        _bossRoomController.SendMessage("HandleBossDeath");

        Assert.IsFalse(_closedVent.activeSelf);
        Assert.IsTrue(_openVent.activeSelf);
    }
}
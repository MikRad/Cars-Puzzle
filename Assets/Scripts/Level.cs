using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int _movesLimit = 5;

    public int MovesLimit => _movesLimit;
}

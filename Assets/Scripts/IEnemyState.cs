public interface IEnemyState
{
    //상태에 진입할 때 1회 호출
    void EnterState();
    //해당 상태에 머무르는 동안 매 프레임 호출
    void UpdateState();
    //해당 상태에 머무르는 동안 물리 연산을 담당하는 함수(고정 프레임)
    void FixedUpdateState();
    //상태에서 벗어났을 때 호출
    void ExitState();
}
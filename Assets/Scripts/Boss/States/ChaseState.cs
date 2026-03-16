using UnityEngine;

public class ChaseState : IBossState
{
    private Vector2 smoothedSteering;

    public void EnterState(BossContext context)
    {
        smoothedSteering = Vector2.zero;
    }

    public void UpdateState(BossContext context)
    {
        if (context.IsDead)
        {
            return;
        }

        if (TryTransitionToAttack(context))
        {
            return;
        }

        if (TryTransitionToDash(context))
        {
            return;
        }

        ChasePlayer(context);
    }

    private bool TryTransitionToAttack(BossContext context)
    {
        float distanceToPlayer = GetDistanceToPlayer(context);
        if (distanceToPlayer <= context.Data.attackRange)
        {
            context.Brain.ChangeState(new AttackState());
            return true;
        }
        return false;
    }

    private bool TryTransitionToDash(BossContext context)
    {
        float distanceToPlayer = GetDistanceToPlayer(context);
        bool isCooldownOver = Time.time >= context.LastDashTime + context.Data.dashCooldown;
        bool isCloseEnough = distanceToPlayer <= context.Data.dashTriggerDistance;
        bool shouldDash = Random.value <= context.Data.dashChance;

        if (isCooldownOver && isCloseEnough && shouldDash)
        {
            context.Brain.ChangeState(new DashState());
            return true;
        }
        return false;
    }

    private void ChasePlayer(BossContext context)
    {
        Vector2 desiredDirection = GetDirectionToPlayer(context);
        Vector2 avoidanceForce = CalculateAvoidance(context, desiredDirection);
        
        Vector2 finalSteering = desiredDirection + avoidanceForce;
        
        ApplyMovement(context, finalSteering);
    }

    private Vector2 CalculateAvoidance(BossContext context, Vector2 desiredDirection)
    {
        Vector2 avoidance = Vector2.zero;

        avoidance += GetAvoidanceFromCircleCast(context, desiredDirection, context.Data.wallCheckDistance, 1f);

        Vector2 leftDirection = Quaternion.Euler(0, 0, 45f) * desiredDirection;
        avoidance += GetAvoidanceFromCircleCast(context, leftDirection, context.Data.wallCheckDistance * 0.8f, 0.7f);

        Vector2 rightDirection = Quaternion.Euler(0, 0, -45f) * desiredDirection;
        avoidance += GetAvoidanceFromCircleCast(context, rightDirection, context.Data.wallCheckDistance * 0.8f, 0.7f);

        return avoidance;
    }

    private Vector2 GetAvoidanceFromCircleCast(BossContext context, Vector2 direction, float distance, float weight)
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            context.BossTransform.position,
            context.Data.bodyRadius,
            direction,
            distance,
            context.Data.wallLayer
        );

        if (hit.collider != null)
        {
            Vector2 wallNormal = hit.normal;
            Vector2 slideDirection = new Vector2(-wallNormal.y, wallNormal.x);
            
            float alignment = Vector2.Dot(slideDirection, direction);

            if (Mathf.Abs(alignment) < 0.1f)
            {
                alignment = Vector2.Dot(slideDirection, context.LastMoveDirection);
                
                if (Mathf.Abs(alignment) < 0.1f)
                {
                    alignment = 1f; 
                }
            }

            if (alignment < 0)
            {
                slideDirection = -slideDirection;
            }

            return (wallNormal * 0.8f + slideDirection * 1.5f) * weight;
        }

        return Vector2.zero;
    }

    private void ApplyMovement(BossContext context, Vector2 steering)
    {
        smoothedSteering = Vector2.Lerp(smoothedSteering, steering, context.Data.steeringSmooth);

        if (smoothedSteering.sqrMagnitude > 1f)
        {
            smoothedSteering.Normalize();
        }

        context.LastMoveDirection = smoothedSteering;

        context.Animator.SetFloat("MoveX", smoothedSteering.x);
        context.Animator.SetFloat("MoveY", smoothedSteering.y);

        context.Movement.Move(smoothedSteering, context.Data.speed);
    }

    private float GetDistanceToPlayer(BossContext context)
    {
        return Vector2.Distance(context.BossTransform.position, context.Player.position);
    }

    private Vector2 GetDirectionToPlayer(BossContext context)
    {
        return (context.Player.position - context.BossTransform.position).normalized;
    }

    public void ExitState(BossContext context)
    {
        context.Movement.Stop();
    }
}
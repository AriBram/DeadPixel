using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


public class AnimatedEnemy : Enemy {

    public SkeletonGraphic enemyAnim;


    public override void Init(EnemyType eType, MovementPoint point) {
        base.Init(eType, point);

        InitializeAnimation();

        colorData = Field.Instance.GetRandomColor();
        SetSkeletonColor(colorData.qType);

        SetOrientation();

        hasShield = false;
        shield.gameObject.SetActive(false);

        healthPoints = Random.Range(minHP, maxHP + 1);
        hpCaption.text = healthPoints.ToString();
    }
    
    
    public void InitializeAnimation() {
        enemyAnim.Initialize(true);
        enemyAnim.AnimationState.SetAnimation(0, "idle", true);
    }

    public void SetSkeletonColor(QBitType qType) {
        switch(qType) {
            case QBitType.GREEN:
                enemyAnim.Skeleton.SetSkin("green");
                activateIndicator.Skeleton.SetSkin("green");
                break;
            case QBitType.BLUE:
                enemyAnim.Skeleton.SetSkin("blue");
                activateIndicator.Skeleton.SetSkin("blue");
                break;
            case QBitType.RED:
                enemyAnim.Skeleton.SetSkin("red");
                activateIndicator.Skeleton.SetSkin("red");
                break;
        }

        enemyAnim.Skeleton.SetSlotsToSetupPose();
        enemyAnim.LateUpdate();
    }

    public void SetOneShotAnimation(string animName) {
        enemyAnim.AnimationState.SetAnimation(0, animName, false);
        enemyAnim.AnimationState.AddAnimation(0, "idle", true, 0);
    }

    public void SetLoopAnimation(string animName) {
        enemyAnim.AnimationState.SetAnimation(0, animName, true);
    }





    public void SetAttackAnimation(MovementDirection mDir) {
        switch(mDir) {
            case MovementDirection.Up: case MovementDirection.Down:
                SetOneShotAnimation("attack_vertical");
                break;
            case MovementDirection.Right: case MovementDirection.Up_Right: case MovementDirection.Down_Right:
                FlipHorizontalToLeft();
                SetOneShotAnimation("attack_horizontal");
                break;
            case MovementDirection.Left: case MovementDirection.Up_Left: case MovementDirection.Down_Left:
                FlipHorizontalToRight();
                SetOneShotAnimation("attack_horizontal");
                break;
        }
    }



    public void FlipHorizontalToLeft() {
        enemyAnim.Skeleton.ScaleX = -1;
    }

    public void FlipHorizontalToRight() {
        enemyAnim.Skeleton.ScaleX = 1;
    }


    public void SetOrientation() {
        MovementDirection newDir = Player.Instance.input.GetMovemenetDirection(currentPoint, PlayerController.Instance.currentPoint);
        switch(newDir) {
            case MovementDirection.Right: case MovementDirection.Up_Right: case MovementDirection.Down_Right:
                FlipHorizontalToLeft();
                break;
            case MovementDirection.Left: case MovementDirection.Up_Left: case MovementDirection.Down_Left:
                FlipHorizontalToRight();
                break;
        }
    }



    public override void ActivateMove() {
        SetOrientation();
        enemyAnim.AnimationState.SetAnimation(0, "idle", true);
        base.ActivateMove();
    }

    public override void GetDamageByPlayer(int damage, QBitType qType) {
        if(hasShield && qType == colorData.qType)
            return;

        healthPoints -= damage;
        if(healthPoints <= 0) {
            MovementPoint point = MovementManager.Instance.Points.Find(p => p.x == currentPoint.x && p.y == currentPoint.y);
            point.Reset();
            Field.Instance.enemiesItems.Remove(this);
            Field.Instance.deathsCounter[eType] += 1;
            Player.Instance.enemiesKilled[eType] += 1;

            enemyAnim.AnimationState.SetAnimation(0, "idle", false);
            enemyAnim.AnimationState.AddAnimation(0, "death", false, 0).Complete += e => Destroy(this.gameObject);
        }
        else
            SetOneShotAnimation("get_damage");

        hpCaption.text = healthPoints.ToString();
    }

    public override void Attack() {
        MovementPoint playerPoint = PlayerController.Instance.currentPoint;
        bool isPlayerInAttackRadius = attackPoints.Find(ap => ap.x == playerPoint.x && ap.y == playerPoint.y) == null ? false : true;
        QBitType playerQType = Player.Instance.colorType;

        if(isPlayerInAttackRadius && colorData.qType != playerQType) {
            Player.Instance.health.GetDamage(attackPower);
            MovementDirection mDir = Player.Instance.input.GetMovemenetDirection(currentPoint, PlayerController.Instance.currentPoint);
            StartCoroutine(StartAttack(mDir));
        }
        else
            onMoveEnd.Invoke();
    }

    public IEnumerator StartAttack(MovementDirection mDir) {
        SetAttackAnimation(mDir);

        yield return new WaitForSeconds(0.4f);

        onMoveEnd.Invoke();
    }




    public override void SetActive(bool isActive) {
        activateIndicator.gameObject.SetActive(isActive);
        
        if(isActive) {
            enemyAnim.AnimationState.SetAnimation(0, "fear", true);
        }
        else {
            enemyAnim.AnimationState.SetAnimation(0, "idle", true);
        }
    }
}

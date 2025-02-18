using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy;
using Interfaces;

namespace States
{
    public class ShootState : ICharacterState {
        private readonly PlayerController _playerController;
        private readonly IArrowPool _arrowPool;
        private readonly IEnemyManager _enemyManager;
        private CancellationTokenSource _attackCTS;

        // Bu alanlar AttackRoutine'da hesaplanan parametreleri saklar.
        private int currentBaseArrowCount;
        private float currentComputedAttackSpeed;

        public ShootState(PlayerController playerController, IArrowPool arrowPool, IEnemyManager enemyManager) {
            _playerController = playerController;
            _arrowPool = arrowPool;
            _enemyManager = enemyManager;
        }

        public void Enter() {
            _playerController.AnimationHandler.SetAttack(true);
            _attackCTS = new CancellationTokenSource();
            // ShootState'e girerken oyuncunun Y rotasyonunu yavaşça 90°'ye getir.
            SmoothRotateToY90(_attackCTS.Token).Forget();
            AttackRoutineAsync(_attackCTS.Token).Forget();
        }

        public void Exit() {
            if (_attackCTS != null) {
                _attackCTS.Cancel();
                _attackCTS.Dispose();
            }
            _playerController.AnimationHandler.SetAttack(false);
        }

        public void OnTick() {
            // Gerekirse ek durum güncellemeleri yapılabilir.
        }

        // Bu metod, periyodik olarak attack parametrelerini hesaplar.
        private async UniTaskVoid AttackRoutineAsync(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                float attackSpeedFactor = _playerController.playerSkills.attackSpeedIncrease 
                    ? (_playerController.playerSkills.rageMode ? 4f : 2f) 
                    : 1f;
                currentComputedAttackSpeed = _playerController.baseAttackSpeed * attackSpeedFactor;
                currentBaseArrowCount = _playerController.playerSkills.arrowMultiplication ? 2 : 1;
                if (_playerController.playerSkills.rageMode) {
                    currentBaseArrowCount = _playerController.playerSkills.arrowMultiplication ? 4 : 2;
                }
                // Bu noktada attack animasyonu oynatılmalı; animasyon event'i ok spawn'u tetikleyecek.
                // (Animasyon içerisinde Attack bool'una bağlı geçişler ayarlanmalı.)
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
            }
        }

        // Animasyon event'i tetiklendiğinde bu metod çağrılır.
        public void HandleAttackAnimationEvent() {
            FireArrows(currentBaseArrowCount);
            if (currentComputedAttackSpeed > 10f) {
                int extraMultiplier = Mathf.FloorToInt(currentComputedAttackSpeed / 10f);
                int extraShots = currentBaseArrowCount * extraMultiplier;
                SpawnExtraArrows(extraShots);
            }
        }

        private async void SpawnExtraArrows(int extraShots) {
            float shotInterval = 0.2f;
            for (int i = 0; i < extraShots; i++) {
                await UniTask.Delay(TimeSpan.FromSeconds(shotInterval));
                FireArrows(currentBaseArrowCount);
            }
        }

        private void FireArrows(int arrowCount) {
            // Oklar, oyuncunun ArrowTransform noktasından spawn olacak.
            var spawnPoint = _playerController.ArrowTransform.position;
            // Hedef olarak en yakın enemy belirleniyor.
            EnemyBase target = _enemyManager.GetNearestEnemy(_playerController.transform.position);
            if (target == null) return;
            Vector3 targetDir = (target.transform.position - spawnPoint).normalized;
            for (int i = 0; i < arrowCount; i++) {
                Vector3 finalDir = targetDir;
                if (arrowCount > 1) {
                    float angleOffset = (i - (arrowCount - 1) / 2f) * 5f; // 5° spread örneği
                    finalDir = Quaternion.Euler(0, angleOffset, 0) * targetDir;
                }
                var arrow = _arrowPool.GetArrow();
                float arrowSpeed = 20f;
                float arrowDamage = 10f;
                arrow.Initialize(spawnPoint, finalDir, arrowSpeed, arrowDamage,
                    _playerController.playerSkills.bounceDamage,
                    _playerController.playerSkills.rageMode ? 2 : 1,
                    _playerController.playerSkills.burnDamage,
                    _playerController.playerSkills.rageMode ? 6f : 3f);
                Debug.DrawRay(spawnPoint, finalDir * 5f, Color.red, 1f);
            }
        }

        // Oyuncu ShootState'e girerken Y rotasyonunu yavaşça 90°'ye getirir.
        private async UniTask SmoothRotateToY90(CancellationToken token) {
            Quaternion targetRotation = Quaternion.Euler(
                _playerController.transform.eulerAngles.x, 
                90f, 
                _playerController.transform.eulerAngles.z);
            while (!token.IsCancellationRequested && 
                   Quaternion.Angle(_playerController.transform.rotation, targetRotation) > 0.1f) {
                _playerController.transform.rotation = Quaternion.Slerp(
                    _playerController.transform.rotation, 
                    targetRotation, 
                    Time.unscaledDeltaTime * _playerController.rotationSpeed);  // unscaledDeltaTime kullanıldı.
                await UniTask.Yield(token);
            }
            _playerController.transform.rotation = targetRotation;
        }
    }
}

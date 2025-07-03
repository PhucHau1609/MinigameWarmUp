using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace EasyUI.PickerWheelUI {
    public class RewardDisplay : MonoBehaviour {
        [Header("Reward Display Settings")]
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private Image rewardIcon;
        [SerializeField] private TextMeshProUGUI rewardLabel;
        [SerializeField] private Text rewardAmount;
        
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 1.5f;
        [SerializeField] private float scaleMultiplier = 1.3f;
        [SerializeField] private int bounceCount = 3;
        [SerializeField] private float delayBeforeShow = 0.5f;
        [SerializeField] private float bounceSpeed = 0.6f; // Tốc độ của mỗi bounce
        
        private bool isRewardShowing = false;
        private Sequence animationSequence;
        
        void Start() {
            // Đảm bảo reward panel bắt đầu ở trạng thái ẩn
            if (rewardPanel != null) {
                rewardPanel.SetActive(false);
            }
        }
        
        void Update() {
            // Kiểm tra click chuột trái để tắt reward display
            if (isRewardShowing && Input.GetMouseButtonDown(0)) {
                HideReward();
            }
        }
        
        public void ShowReward(WheelPiece piece) {
            if (rewardPanel == null || rewardIcon == null) {
                Debug.LogError("RewardDisplay: Missing required UI components!");
                return;
            }
            
            // Cập nhật thông tin phần thưởng
            rewardIcon.sprite = piece.Icon;
            
            if (rewardLabel != null) {
                rewardLabel.text = piece.Label;
            }
            
            if (rewardAmount != null) {
                rewardAmount.text = piece.Amount.ToString();
            }
            
            // Hiển thị panel sau một khoảng delay
            DOVirtual.DelayedCall(delayBeforeShow, () => {
                DisplayRewardWithAnimation();
            });
        }
        
        private void DisplayRewardWithAnimation() {
            if (rewardPanel == null) return;
            
            // Kích hoạt panel
            rewardPanel.SetActive(true);
            isRewardShowing = true;
            
            // Reset scale và rotation về trạng thái ban đầu
            rewardIcon.transform.localScale = Vector3.zero;
            rewardIcon.transform.rotation = Quaternion.identity;
            
            // Tạo sequence animation mới
            animationSequence = DOTween.Sequence();
            
            // Animation xuất hiện đầu tiên - từ 0 lên 1
            animationSequence.Append(rewardIcon.transform.DOScale(1f, bounceSpeed)
                                   .SetEase(Ease.OutBack));
            
            // Thêm delay nhỏ giữa các bounce
            animationSequence.AppendInterval(0.1f);
            
            // Animation bounce nhiều lần với nhịp độ đều
            for (int i = 0; i < bounceCount; i++) {
                // Phóng to
                animationSequence.Append(rewardIcon.transform.DOScale(scaleMultiplier, bounceSpeed / 2)
                                       .SetEase(Ease.OutQuad));
                
                // Thu nhỏ về bình thường
                animationSequence.Append(rewardIcon.transform.DOScale(1f, bounceSpeed / 2)
                                       .SetEase(Ease.InQuad));
                
                // Thêm delay nhỏ giữa các bounce (trừ lần cuối)
                if (i < bounceCount - 1) {
                    animationSequence.AppendInterval(0.1f);
                }
            }
            
            // Animation xoay nhẹ song song (tùy chọn - có thể tắt)
            // animationSequence.Join(rewardPanel.transform.DORotate(new Vector3(0, 0, 10), animationDuration)
            //                      .SetLoops(2, LoopType.Yoyo)
            //                      .SetEase(Ease.InOutSine));
        }
        
        private void HideReward() {
            if (!isRewardShowing) return;
            
            // Dừng animation hiện tại nếu có
            if (animationSequence != null && animationSequence.IsActive()) {
                animationSequence.Kill();
            }

            rewardPanel.SetActive(false);
            
            // Animation ẩn mượt mà
            rewardPanel.transform.DOScale(0f, 0.4f)
                       .SetEase(Ease.InBack)
                       .OnComplete(() => {
                           isRewardShowing = false;
                           
                           // Reset về trạng thái ban đầu
                           rewardIcon.transform.localScale = Vector3.one;
                           rewardIcon.transform.rotation = Quaternion.identity;
                       });
        }
        
        // Method để tắt reward từ code khác nếu cần
        public void ForceHideReward() {
            HideReward();
        }
        
        // Kiểm tra xem reward có đang hiển thị không
        public bool IsRewardShowing() {
            return isRewardShowing;
        }
    }
}
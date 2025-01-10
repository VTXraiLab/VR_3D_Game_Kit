using System.Collections;
using UnityEngine;

namespace Gamekit3D
{
    public class HealthUI : MonoBehaviour
    {
        public Damageable representedDamageable;
        public GameObject healthIconPrefab;

        protected Animator[] m_HealthIconAnimators;

        protected readonly int m_HashActivePara = Animator.StringToHash("Active");
        protected readonly int m_HashInactiveState = Animator.StringToHash("Inactive");
        protected const float k_HeartIconAnchorWidth = 0.328f; // modded code

        IEnumerator Start()
        {
            if (representedDamageable == null)
                yield break;

            yield return null;

            m_HealthIconAnimators = new Animator[representedDamageable.maxHitPoints];

            for (int i = 0; i < representedDamageable.maxHitPoints; i++)
            {
                GameObject healthIcon = Instantiate(healthIconPrefab);
                healthIcon.transform.SetParent(transform);
                healthIcon.transform.Rotate(0,90f,0); // modded code
                RectTransform healthIconRect = healthIcon.transform as RectTransform;
                healthIconRect.anchoredPosition = Vector2.zero;
                healthIcon.transform.localPosition = new Vector3(0, 0, 0.67f); // new code
                healthIcon.transform.eulerAngles = new Vector3(0f,0f,135f); // new code
                // had to resize HeartIcon
                healthIconRect.sizeDelta = Vector2.zero;
                healthIconRect.anchorMin += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                healthIconRect.anchorMax += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                m_HealthIconAnimators[i] = healthIcon.GetComponent<Animator>();

                if (representedDamageable.currentHitPoints < i + 1)
                {
                    m_HealthIconAnimators[i].Play(m_HashInactiveState);
                    m_HealthIconAnimators[i].SetBool(m_HashActivePara, true); // modded to true
                }
            }
        }

        public void ChangeHitPointUI(Damageable damageable)
        {
            if (m_HealthIconAnimators == null)
                return;

            for (int i = 0; i < m_HealthIconAnimators.Length; i++)
            {
                m_HealthIconAnimators[i].SetBool(m_HashActivePara, damageable.currentHitPoints >= i + 1);
            }
        }
    } 
}

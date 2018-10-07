using Hertzole.GoldPlayer.Core;
using System.Collections.Generic;
using UnityEngine;
#if HERTZLIB_UPDATE_MANAGER
using Hertzole.HertzLib;
#endif

namespace Hertzole.GoldPlayer.Weapons
{
    [AddComponentMenu("Gold Player/Weapons/Gold Player Weapon Manager")]
#if HERTZLIB_UPDATE_MANAGER
    public class GoldPlayerWeapons : PlayerBehaviour, IUpdate
#else
    public class GoldPlayerWeapons : PlayerBehaviour
#endif
    {
        [SerializeField]
        private GoldPlayerWeapon[] m_AvailableWeapons;
        public GoldPlayerWeapon[] AvailableWeapons { get { return m_AvailableWeapons; } set { m_AvailableWeapons = value; } }
        [SerializeField]
        private LayerMask m_HitLayer;
        public LayerMask HitLayer { get { return m_HitLayer; } set { m_HitLayer = value; } }

#if UNITY_EDITOR
        [Header("Change Weapon Settings")]
#endif
        [SerializeField]
        private bool m_CanChangeWeapon = true;
        public bool CanChangeWeapon { get { return m_CanChangeWeapon; } set { m_CanChangeWeapon = value; } }
        [SerializeField]
        private bool m_CanScrollThrough = true;
        public bool CanScrollThrough { get { return m_CanScrollThrough; } set { m_CanScrollThrough = value; } }
        [SerializeField]
        private bool m_LoopScroll = true;
        public bool LoopScroll { get { return m_LoopScroll; } set { m_LoopScroll = value; } }
        [SerializeField]
        private bool m_InvertScroll = false;
        public bool InvertScroll { get { return m_InvertScroll; } set { m_InvertScroll = value; } }
        [SerializeField]
        private bool m_EnableScrollDelay = false;
        public bool EnableScrollDelay { get { return m_EnableScrollDelay; } set { m_EnableScrollDelay = value; } }
        [SerializeField]
        private float m_ScrollDelay = 0.1f;
        public float ScrollDelay { get { return m_ScrollDelay; } set { m_ScrollDelay = value; } }
        [SerializeField]
        private bool m_CanUseNumberKeys = true;
        public bool CanUseNumberKeys { get { return m_CanUseNumberKeys; } set { m_CanUseNumberKeys = value; } }

        private int m_NewWeaponIndex = -1;
        protected int m_CurrentWeaponIndex = -1;

        protected bool m_DoPrimaryAttack = false;
        protected bool m_DoSecondaryAttack = false;

        protected float m_NextScroll = 0;

        private List<int> m_MyWeapons = new List<int>();

        protected GoldPlayerWeapon m_PreviousWeapon = null;

        public GoldPlayerWeapon CurrentWeapon
        {
            get
            {
                if (m_MyWeapons == null || m_MyWeapons.Count == 0)
                    return null;

                return m_CurrentWeaponIndex < 0 || m_CurrentWeaponIndex > m_MyWeapons.Count - 1 ? null : m_AvailableWeapons[m_MyWeapons[m_CurrentWeaponIndex]];
            }
        }

        public delegate void WeaponChangeEvent(GoldPlayerWeapon previousWeapon, GoldPlayerWeapon newWeapon);
        public event WeaponChangeEvent OnWeaponChanged;

#if HERTZLIB_UPDATE_MANAGER
        protected virtual void OnEnable()
        {
            UpdateManager.AddUpdate(this);
        }

        protected virtual OnDisable()
        {
            UpdateManager.RemoveUpdate(this);
        }
#endif

        protected virtual void Start()
        {
            for (int i = 0; i < m_AvailableWeapons.Length; i++)
            {
                m_AvailableWeapons[i].gameObject.SetActive(false);
                m_AvailableWeapons[i].Initialize(m_HitLayer);
            }

            //TODO: Add a way in the editor to set what weapons are included.
            AddWeapon(0);
            AddWeapon(1);
            AddWeapon(2);

            ChangeWeapon(0);
        }

#if HERTZLIB_UPDATE_MANAGER
        public virtual void OnUpdate()
#else
        protected virtual void Update()
#endif
        {
            HandleWeaponChanging();
            HandlePrimaryAttacking();
            HandleSecondaryAttacking();
            HandleReloading();
        }

        protected virtual void HandleWeaponChanging()
        {
            if (!m_CanChangeWeapon)
                return;

            if (m_CanUseNumberKeys)
            {
                if (GetButtonDown("Change Weapon 1", KeyCode.Alpha1))
                    ChangeWeapon(0);
                if (GetButtonDown("Change Weapon 2", KeyCode.Alpha2))
                    ChangeWeapon(1);
                if (GetButtonDown("Change Weapon 3", KeyCode.Alpha3))
                    ChangeWeapon(2);
                if (GetButtonDown("Change Weapon 4", KeyCode.Alpha4))
                    ChangeWeapon(3);
                if (GetButtonDown("Change Weapon 5", KeyCode.Alpha5))
                    ChangeWeapon(4);
                if (GetButtonDown("Change Weapon 6", KeyCode.Alpha6))
                    ChangeWeapon(5);
                if (GetButtonDown("Change Weapon 7", KeyCode.Alpha7))
                    ChangeWeapon(6);
                if (GetButtonDown("Change Weapon 8", KeyCode.Alpha8))
                    ChangeWeapon(7);
                if (GetButtonDown("Change Weapon 9", KeyCode.Alpha9))
                    ChangeWeapon(8);
            }

            if (m_CanScrollThrough)
            {
                if (GetAxis("Change Weapon Scroll", "Mouse ScrollWheel") >= 0.1f)
                {
                    if (m_EnableScrollDelay && Time.time < m_NextScroll)
                        return;

                    if (!m_InvertScroll)
                        GoUpWeaponList();
                    else
                        GoDownWeaponList();

                    if (m_EnableScrollDelay)
                        m_NextScroll = Time.time + m_ScrollDelay;
                }

                if (GetAxis("Change Weapon Scroll", "Mouse ScrollWheel") <= -0.1f)
                {
                    if (m_EnableScrollDelay && Time.time < m_NextScroll)
                        return;

                    if (!m_InvertScroll)
                        GoDownWeaponList();
                    else
                        GoUpWeaponList();

                    if (m_EnableScrollDelay)
                        m_NextScroll = Time.time + m_ScrollDelay;
                }
            }
        }

        protected virtual void HandlePrimaryAttacking()
        {
            if (CurrentWeapon == null)
                return;

            switch (CurrentWeapon.PrimaryTriggerType)
            {
                case GoldPlayerWeapon.TriggerTypeEnum.Manual:
                    m_DoPrimaryAttack = GetButtonDown("Primary Attack");
                    break;
                case GoldPlayerWeapon.TriggerTypeEnum.Automatic:
                    m_DoPrimaryAttack = GetButton("Primary Attack");
                    break;
                default:
                    throw new System.NotImplementedException("No support for " + CurrentWeapon.PrimaryTriggerType + " trigger type!");
            }

            if (m_DoPrimaryAttack)
                CurrentWeapon.PrimaryAttack();
        }

        protected virtual void HandleSecondaryAttacking()
        {
            if (CurrentWeapon == null)
                return;

            switch (CurrentWeapon.SecondaryTriggerType)
            {
                case GoldPlayerWeapon.TriggerTypeEnum.Manual:
                    m_DoSecondaryAttack = GetButtonDown("Secondary Attack");
                    break;
                case GoldPlayerWeapon.TriggerTypeEnum.Automatic:
                    m_DoSecondaryAttack = GetButton("Secondary Attack");
                    break;
                default:
                    throw new System.NotImplementedException("No support for " + CurrentWeapon.PrimaryTriggerType + " trigger type!");
            }

            if (m_DoSecondaryAttack)
                CurrentWeapon.SecondaryAttack();
        }

        protected virtual void HandleReloading()
        {
            if (CurrentWeapon == null)
                return;

            if (GetButtonDown("Reload"))
            {
                CurrentWeapon.Reload();
            }
        }

        protected void GoUpWeaponList()
        {
            if (m_MyWeapons == null || m_MyWeapons.Count == 0)
                return;

            m_NewWeaponIndex = m_CurrentWeaponIndex - 1;
            if (m_NewWeaponIndex < 0)
            {
                if (m_LoopScroll)
                    m_NewWeaponIndex = m_MyWeapons.Count - 1;
            }

            ChangeWeapon(m_NewWeaponIndex);
        }

        protected void GoDownWeaponList()
        {
            if (m_MyWeapons == null || m_MyWeapons.Count == 0)
                return;

            m_NewWeaponIndex = m_CurrentWeaponIndex + 1;
            if (m_NewWeaponIndex > m_MyWeapons.Count - 1)
            {
                if (m_LoopScroll)
                    m_NewWeaponIndex = 0;
            }

            ChangeWeapon(m_NewWeaponIndex);
        }

        public virtual void AddWeapon(GoldPlayerWeapon weapon)
        {
            for (int i = 0; i < m_AvailableWeapons.Length; i++)
            {
                if (m_AvailableWeapons[i] == weapon)
                {
                    AddWeapon(i);
                    return;
                }
            }

            throw new System.ArgumentException(weapon.gameObject.name + " has not been added to the list of available weapons!");
        }

        public virtual void AddWeapon(int weaponIndex)
        {
            if (m_MyWeapons.Contains(weaponIndex))
            {
                Debug.LogWarning("Weapon with index " + weaponIndex + " already exists in the player weapon inventory!");
                return;
            }

            m_MyWeapons.Add(weaponIndex);
        }

        public virtual void RemoveWeapon(GoldPlayerWeapon weapon)
        {
            int weaponToRemoveIndex = -1;
            for (int i = 0; i < m_AvailableWeapons.Length; i++)
            {
                if (m_AvailableWeapons[i] == weapon)
                {
                    weaponToRemoveIndex = i;
                    break;
                }
            }

            if (weaponToRemoveIndex < 0)
            {
                throw new System.ArgumentException(weapon + " has not been added to the available weapons list!");
            }

            RemoveWeapon(weaponToRemoveIndex);
        }

        public virtual void RemoveWeapon(int weaponIndex)
        {
            if (!m_MyWeapons.Contains(weaponIndex))
            {
                Debug.LogWarning("No weapon with the index " + weaponIndex + " exists in the player weapon inventory!");
                return;
            }
        }

        public virtual void ChangeWeapon(int index)
        {
            if (m_AvailableWeapons == null || m_AvailableWeapons.Length == 0 || m_CurrentWeaponIndex == index)
                return;

            if (index < 0)
                index = 0;
            else if (index > m_AvailableWeapons.Length - 1)
                index = m_AvailableWeapons.Length - 1;

            if (CurrentWeapon != null)
            {
                CurrentWeapon.gameObject.SetActive(false);
                CurrentWeapon.OnPutAway();
            }

            m_PreviousWeapon = CurrentWeapon;

            m_CurrentWeaponIndex = index;
            if (CurrentWeapon != null)
            {
                CurrentWeapon.gameObject.SetActive(true);
                CurrentWeapon.OnEquip();
            }

#if NET_4_6 || UNITY_2018_3_OR_NEWER
            OnWeaponChanged?.Invoke(m_PreviousWeapon, CurrentWeapon);
#else
            if (OnWeaponChanged != null)
                OnWeaponChanged.Invoke(m_PreviousWeapon, CurrentWeapon);
#endif
        }
    }
}

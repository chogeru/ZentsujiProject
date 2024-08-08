using UnityEngine;
using AbubuResouse.Log;


namespace AbubuResouse.Singleton
{
    /// <summary>
    /// �V���O���g���p�^�[�����������邽�߂̒��ۃN���X
    /// </summary>
    /// <typeparam name="T">�V���O���g���Ƃ��Ďg�p����N���X�̌^</typeparam>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {

        private static T _instance;

        /// <summary>
        /// �V���O���g���̃C���X�^���X���擾
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        DebugUtility.LogError($"{typeof(T).Name}�̃C���X�^���X���V�[���ɑ��݂��Ȃ��I");
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g�̏��������ɌĂяo�����
        /// �V���O���g���̃C���X�^���X��ݒ肵�A�I�u�W�F�N�g���j������Ȃ��悤�ɂ���
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                DebugUtility.Log($"{typeof(T).Name}�̃C���X�^���X�����łɑ��݂��Ă���I");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g���j�������ۂɌĂяo��
        /// �C���X�^���X���N���A����
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                DebugUtility.Log($"{typeof(T).Name}�̃C���X�^���X��j���I");
            }
        }
    }
}
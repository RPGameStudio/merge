using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using DG.Tweening;
using RX;
using UnityEngine;

namespace Gameplay
{
    public struct EnemyViewData
    {
        public EntityState State;
        public Action<EntityView> OnBeginDrag;
        public Action<EntityView> OnDrag;
        public Action<EntityView> OnEndDrag;
        public Action<EntityView> OnClick;
    }

    [RequireComponent(typeof(DragBehaviour), typeof(ClickBehaviour), typeof(SpriteRenderer))]
    public class EntityView : MonoBehaviour
    {
        [SerializeField] private DragBehaviour _dragBehaviuor;
        [SerializeField] private ClickBehaviour _clickBehaviour;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public EntityState State { get; private set; }

        public void Initialize(EnemyViewData initialData)
        {
            State = initialData.State;
            _dragBehaviuor.OnBeginDrag += OnBeginDrag;
            _dragBehaviuor.OnEndDrag += OnEndDrag;
            _dragBehaviuor.OnDrag += OnDrag;
            _clickBehaviour.Initialize(() => initialData.OnClick(this));


            void OnBeginDrag()
            {
                _spriteRenderer.color = Color.red;
                _spriteRenderer.sortingOrder += 100;
                initialData.OnBeginDrag?.Invoke(this);
            }

            void OnEndDrag()
            {
                _spriteRenderer.color = Color.white;
                _spriteRenderer.sortingOrder -= 100;
                initialData.OnEndDrag?.Invoke(this);
            }

            void OnDrag()
            {
                initialData.OnDrag?.Invoke(this);
            }
        }

        public async Task<Vector2> VisualMergeWith(EntityView view)
        {
            var v = transform.position - view.transform.position;
            var halfv = v / 2;
            var middle = view.transform.position + halfv;

            await DOTween.Sequence()
            .Append(transform.DOMove(middle, 0.3f))
            .Join(view.transform.DOMove(middle, 0.3f))
            .AppendInterval(0.3f)
            .Join(transform.DOScale(Vector3.zero, 0.3f))
            .Join(view.transform.DOScale(Vector3.zero, 0.3f))
            .AppendCallback(() =>
            {
                Destroy(gameObject);
                Destroy(view.gameObject);
            }).AsyncWaitForCompletion();

            return middle;
        }

        private void OnDestroy() => _dragBehaviuor.Reset();

        private void Reset()
        {
            _dragBehaviuor = GetComponent<DragBehaviour>();
            _clickBehaviour = GetComponent<ClickBehaviour>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
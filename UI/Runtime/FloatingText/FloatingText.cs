//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//using TextField = TMPro.TextMeshProUGUI;

//namespace NorskaLib.UI
//{
//    public class FloatingText : MonoBehaviour
//    {
//        [SerializeField] RectTransform rect;
//        public RectTransform Rect => rect;

//        [SerializeField] RectTransform groupRect;
//        [SerializeField] Graphic[] graphicalElements;

//        [Space]
//        [SerializeField] TextField label;
//        public string Text
//        {
//            set => label.text = value;
//        }

//        DoTweenRectHandler groupTransformer;
//        DoTweenGraphicColorizer[] graphicsFaders;
//        private const float StartFromAnchor         = -0.2f;
//        private const float SlowDownOnAnchor        = +0.0f;
//        private const float SlowDownUntillAnchor    = +0.5f;
//        private const float EndOnAnchor             = +1.0f;
//        private const float FastMoveDuration        = 0.2f;
//        private const float SlowMoveDuration        = 1.2f;
//        float x, height;
//        SequenceNode<Vector2>[] moveSequence;
//        SequenceNode<float>[] alphaSequence;

//        public Action<FloatingText> onSequenceFinished = (instance) => { };

//        void Awake()
//        {
//            CashSequence();

//            groupTransformer = new DoTweenRectHandler(groupRect);

//            graphicsFaders = new DoTweenGraphicColorizer[graphicalElements.Length];
//            for (int i = 0; i < graphicsFaders.Length; i++)
//                graphicsFaders[i] = new DoTweenGraphicColorizer(graphicalElements[i]);

//            groupTransformer.OnCompleteCallback += OnSequenceFinishes;
//        }

//        void OnDestroy()
//        {
//            groupTransformer.OnCompleteCallback -= OnSequenceFinishes;

//            groupTransformer?.Stop();

//            for (int i = 0; i < graphicsFaders.Length; i++)
//                graphicsFaders[i]?.Stop();
//        }

//        void Start()
//        {
//            SetInitial();
//        }

//        private void CashSequence()
//        {
//            x = groupRect.anchoredPosition.x;
//            height = groupRect.rect.height;

//            moveSequence = new SequenceNode<Vector2>[]
//            {
//                    new SequenceNode<Vector2>(){
//                        value = new Vector2(x, height * SlowDownOnAnchor),
//                        duration = FastMoveDuration,
//                        ease = DG.Tweening.Ease.Flash
//                    },
//                    new SequenceNode<Vector2>(){
//                        value = new Vector2(x, height * SlowDownUntillAnchor),
//                        duration = SlowMoveDuration,
//                        ease = DG.Tweening.Ease.Linear
//                    },
//                    new SequenceNode<Vector2>(){
//                        value = new Vector2(x, height *EndOnAnchor),
//                        duration = FastMoveDuration,
//                        ease = DG.Tweening.Ease.Flash
//                    },
//            };
//            alphaSequence = new SequenceNode<float>[]
//            {
//                    new SequenceNode<float>(){
//                        value = 1,
//                        duration = FastMoveDuration,
//                        ease = DG.Tweening.Ease.Flash
//                    },
//                    new SequenceNode<float>(){
//                        value = 1,
//                        duration = SlowMoveDuration,
//                        ease = DG.Tweening.Ease.Linear
//                    },
//                    new SequenceNode<float>(){
//                        value = 0,
//                        duration = FastMoveDuration,
//                        ease = DG.Tweening.Ease.Flash
//                    },
//            };
//        }

//        void OnSequenceFinishes()
//        {
//            onSequenceFinished.Invoke(this);
//        }

//        public void SetInitial()
//        {
//            groupTransformer.SetAnchoredPos(new Vector2(x, height * StartFromAnchor));

//            for (int i = 0; i < graphicsFaders.Length; i++)
//                graphicsFaders[i].SetAlpha(0);
//        }

//        public void Animate()
//        {
//            SetInitial();

//            groupTransformer.MoveAnchored(moveSequence);

//            for (int i = 0; i < graphicsFaders.Length; i++)
//                graphicsFaders[i].Transit(alphaSequence);
//        }

//        public void Break()
//        {
//            groupTransformer.Stop(false);

//            for (int i = 0; i < graphicsFaders.Length; i++)
//                graphicsFaders[i].Stop(false);
//        }
//    }
//}

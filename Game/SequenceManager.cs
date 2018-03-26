using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour 
{
    static public SequenceManager main { get; private set; }

    public class SequenceEntry
    {
        public IEnumerator coroutine = null;
        public int priority = 0;
        public float delay = 0.0f;
        public bool isPlaying = false;
    }

    public SequenceEntry current { get; private set; }
    public bool isPlaying { get { return current != null; } }
    List<SequenceEntry> sequenceQueue = new List<SequenceEntry>();
    bool allowAdd = true;

    void Awake()
    {
        main = this;
    }

    void Update()
    {
        foreach (var sequence in sequenceQueue)
        {
            if (sequence.delay > 0.0f)
            {
                sequence.delay -= Time.unscaledDeltaTime;
            }
        }
        PlayNext();
    }

    void OnDisable()
    {
        ClearAll();
    }

    void PlayNext()
    {
        if (isPlaying == false && sequenceQueue.Count > 0)
        {
            SequenceEntry next = null;
            for (int i = 0; i < sequenceQueue.Count; i++)
            {
                var sequence = sequenceQueue[i];
                if (sequence.delay > 0.0f)
                    continue;

                next = sequence;
                sequenceQueue.RemoveAt(i);
                StartCoroutine(CoRunSequence(next));
                return;
            }
        }
    }

    public void ClearAll()
    {
        StopAllCoroutines();
        sequenceQueue.Clear();
        Finish();
        PlayNext();
    }

    public void Finish(System.Action onComplete = null)
    {
        //Game.main.time.Reset();
        //Game.main.cam.Reset();
        //Game.main.ui.ToGame();
        //Game.main.player.inputEnabled = true;
        allowAdd = true;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }

    public SequenceEntry Run(IEnumerator coroutine, int priority = 0, float delay = 0.0f)
    {
        if (enabled == false)
            return null;
        
        var sequence = new SequenceEntry();
        sequence.coroutine = coroutine;
        sequence.priority = priority;
        sequence.delay = delay;
        //Game.Instance.player.inputEnabled = false;

        int index = sequenceQueue.Count;
        if (sequenceQueue.Count > 0)
        {
            while (index-- > 0)
            {
                if (sequenceQueue[index].priority >= priority)
                {
                    break;
                }
            }
            index++;
        }
        if (index < sequenceQueue.Count)
        {
            sequenceQueue.Insert(index, sequence);
        }
        else
        {
            sequenceQueue.Add(sequence);
        }

        if (isPlaying == false)
        {
            PlayNext();
        }

        return sequence;
    }

    IEnumerator CoRunSequence(SequenceEntry sequence)
    {
        current = sequence;
        sequence.isPlaying = true;

        yield return StartCoroutine(sequence.coroutine);

        sequence.isPlaying = false;
        current = null;
    }

    public void RevealTarget(Transform target, System.Action onComplete = null)
    {
        Run(SequenceRevealTarget(target, onComplete));
    }

    public IEnumerator SequenceRevealTarget(Transform target, System.Action onComplete = null)
    {
        Debug.Log("Sequence: Reveal Target: " + target.name);

        MusicPlayer.main.Stop();

        //SoundPlayer.main.Play("adventure_solve_small");

        yield return StartCoroutine(CoRevealStart(target));

        if (onComplete != null)
        {
            onComplete.Invoke();
        }

        yield return new WaitForSecondsRealtime(2.5f);

        MusicPlayer.main.Resume();

        Finish();
    }

    public void RevealSign(Transform target, string message, System.Action onComplete = null)
    {
        Run(SequenceRevealSign(target, message, onComplete), 0, 0.2f);
    }

    public IEnumerator SequenceRevealSign(Transform target, string message, System.Action onComplete = null)
    {
        Debug.Log("Sequence: Reveal Sign: " + target.name);

        yield return StartCoroutine(CoRevealStart(target));

        MusicPlayer.main.Stop();

        //Game.Instance.ui.ToSign(message);

        float msgTime = GetDisplayTime(message);
        yield return StartCoroutine(CoTimeOrPress(msgTime));

        MusicPlayer.main.Resume();

        Finish(onComplete);
    }

    IEnumerator CoRevealStart(Transform target)
    {
        //Game.Instance.time.Freeze();
        //Game.Instance.ui.ToCutscene();

        yield return new WaitForSecondsRealtime(0.5f);

        //Game.Instance.cam.Reveal(target);

        yield return new WaitForSecondsRealtime(0.5f);
    }

    public void FocusTarget(Transform target, System.Action onComplete = null)
    {
        Run(SequenceFocusTarget(target, onComplete), 0, 0.2f);
    }

    public IEnumerator SequenceFocusTarget(Transform target, System.Action onComplete = null)
    {
        Debug.Log("Sequence: Focus Target: " + target.name);

        MusicPlayer.main.Stop();
        //SoundPlayer.main.Play("adventure_solve_big");

        //Game.Instance.time.Freeze();
        //Game.Instance.ui.ToCutscene();
        //Game.Instance.cam.Zoom(target);

        yield return new WaitForSecondsRealtime(1.5f);

       // Game.Instance.time.SlowMo(0.1f);

        yield return new WaitForSecondsRealtime(0.5f);

        MusicPlayer.main.Resume();

        Finish(onComplete);
    }

    public void FollowTarget(Transform target, Vector3 camDir, float maxTime = 10.0f, System.Action onComplete = null)
    {
        Run(SequenceFollowTarget(target, camDir, maxTime, onComplete));
    }

    IEnumerator SequenceFollowTarget(Transform target, Vector3 camDir, float maxTime = 10.0f, System.Action onComplete = null)
    {
        //Game.Instance.ui.ToCutscene();
        //Game.Instance.cam.FollowObject(target, camDir);

        Vector3 lastPos = Vector3.zero;

        maxTime -= 1.0f;

        while (maxTime > 0.0f)
        {
            if (target == null || target.gameObject.activeInHierarchy == false)
                break;

            var pos = target.position;
            var dif = pos - lastPos;
            lastPos = pos;

            if (dif.sqrMagnitude < 0.01f)
                break;

            maxTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(2.0f);

        Finish(onComplete);
    }

    /*
    public void ShowMap(Actor target, System.Action onComplete = null)
    {
        Run(SequenceShowMap(target, onComplete), 1);
    }

    public IEnumerator SequenceShowMap(Actor target, System.Action onComplete = null)
    {
        Game.Instance.time.Freeze();
        Game.Instance.ui.ToMap(target);

        yield return StartCoroutine(CoTimeOrPress(3.0f, 1.0f));

        Finish(onComplete);
    }
    */

    public void Respawn()
    {
        ClearAll();
        Run(SequenceRespawn());
    }

    public IEnumerator SequenceRespawn()
    {
        Debug.Log("Sequence: Respawn.");

        allowAdd = false;

        yield return new WaitForSecondsRealtime(2.0f);
        //Game.Instance.player.Reset();

        Finish();
    }

    public void Talk(TalkConversation conversation, System.Action onComplete = null)
    {
        conversation.isTalking = true;

        var sequences = new List<SequenceEntry>();

        var talkNodes = conversation.talkNodes;
        for (int i=0; i<talkNodes.Length; i++)
        {
            var talkNode = talkNodes[i];
            sequences.Add(Run(SequenceTalkNode(conversation, talkNode), -1));
        }

        Run(WaitForConversation(conversation, sequences.ToArray(), onComplete), -100);
    }

    float GetDisplayTime(string message)
    {
        float msgTime = 1.6f;
        msgTime += (float)message.Length * 0.09f;
        return msgTime;
    }

    IEnumerator WaitForConversation(TalkConversation conversation, SequenceEntry[] sequences, System.Action onComplete = null)
    {
        yield return StartCoroutine(WaitForSequences(sequences));

        conversation.isTalking = false;

        //conversation.actorA.PickMover();
        //conversation.actorB.PickMover();

        Finish(onComplete);
    }

    IEnumerator WaitForSequences(SequenceEntry[] sequences, System.Action onComplete = null)
    {
        while (true)
        {
            bool isFinished = true;
            foreach (var sequence in sequences)
            {
                if (sequence.isPlaying)
                {
                    isFinished = false;
                }
            }
            if (isFinished)
                break;

            yield return new WaitForSecondsRealtime(0.1f);
        }

        Finish(onComplete);
    }

    IEnumerator SequenceTalkNode(TalkConversation conversation, TalkNode talkNode)
    {
        var dif = conversation.actorA.transform.position - conversation.actorB.transform.position;
        dif = dif.normalized * 5.0f;
        conversation.actorA.transform.position = conversation.actorB.transform.position + dif;

        var talker = talkNode.actor;
        if (talker == null)
        {
            talker = conversation.actorA;
        }

        Debug.Log("Sequence: Talk: " + talker.name + ": " + talkNode.speech);

        //Game.Instance.ui.ToSign(talkNode.speech);

        /*
        var talkerMover = talker.SetMover<TalkMover>();
        if (talkerMover != null)
        {
            talkerMover.talking = true;
        }

        int conversantsTalking = 2;

        if (conversation.actorA != talker)
        {
            var moverA = conversation.actorA.SetMover<TalkMover>();
            moverA.talking = false;
            conversation.actorA.SetTarget(talker);
            conversantsTalking--;
        }

        if (conversation.actorB != talker)
        {
            var moverB = conversation.actorB.SetMover<TalkMover>();
            moverB.talking = false;
            conversation.actorB.SetTarget(talker);
            conversantsTalking--;
        }

        if (conversantsTalking == 0)
        {
            talker.SetTarget(conversation.actorA);
        }

        Game.Instance.cam.Talk(talker, talker == conversation.actorA);

        float msgTime = GetDisplayTime(talkNode.speech);
        yield return StartCoroutine(CoTimeOrPress(msgTime));

        if (conversantsTalking == 0)
        {
            talker.PickMover(); 
        }

        if (talkNode.trigger != null)
        {
            talkNode.trigger.Trigger(true);
        }
        */

        yield return null;
    }

    IEnumerator CoTimeOrPress(float time, float minTime = 0.2f)
    {
        time -= minTime;
        yield return new WaitForSecondsRealtime(minTime);

        while (Input.anyKeyDown == false && time > 0.0f)
        {
            time -= Time.unscaledDeltaTime;
            yield return null;
        }
    }
}

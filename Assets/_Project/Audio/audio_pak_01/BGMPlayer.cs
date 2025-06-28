using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour {
  public List<AudioClip> bgmPlaylist;
  private int index = 0;

  void Start() {
    PlayNext();
  }

  public void PlayNext() {
    if (bgmPlaylist.Count == 0) return;
    AudioManager.S.PlayBGM(bgmPlaylist[index]);
    index = (index + 1) % bgmPlaylist.Count;
  }
}
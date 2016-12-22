﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AgariPanel : MonoBehaviour
{
    public TehaiUI tehai;
    public FuuroUI fuuro;

    public Transform omoteDoraRoot;
    public Transform uraDoraRoot;

    public Transform yakuRoot;
    public GameObject yakuItemPrefab;
    public Vector2 yakuItemPosOffset = new Vector2( 60f, -40f);
    public float yakuDisplayTime = 1.0f;

    public UILabel lab_han;
    public UILabel lab_point;

    public float haiOffset = 2f;

    public string YakuLevelColorStr = "0600B3";
    public string YakuHanColorStr = "000000";

    private const int DoraHaisColumn = 5;

    private List<MahjongPai> _omoteDoraHais = new List<MahjongPai>();
    private List<MahjongPai> _uraDoraHais = new List<MahjongPai>();
    private List<UIYakuItem> _yakuItems = new List<UIYakuItem>();


    void Awake()
    {
        yakuItemPrefab.SetActive(false);
    }


    void InitYakuInfo()
    {
        ClearYakuItemList( _yakuItems );

        lab_han.text = "";
        lab_point.text = "";
    }


    void InitDoraHais()
    {
        if( _omoteDoraHais != null ){
            ClearMahjongList( _omoteDoraHais );

            Hai[] allOmoteDora = GameManager.Instance.MahjongMain.Yama.getAllOmoteDoraHais();

            for( int i = 0; i < DoraHaisColumn; i++ )
            {
                Vector3 pos = new Vector3( -i * (MahjongPai.Width+haiOffset), 0f, 0f );
                Hai hai = allOmoteDora[i];

                MahjongPai pai = PlayerUI.CreateMahjongPai(omoteDoraRoot, pos, hai, false);
                _omoteDoraHais.Add( pai );
            }
        }

        if( _uraDoraHais != null ){
            ClearMahjongList( _uraDoraHais );

            Hai[] allUraDora = GameManager.Instance.MahjongMain.Yama.getAllUraDoraHais();

            for( int i = 0; i < DoraHaisColumn; i++ )
            {
                Vector3 pos = new Vector3( -i * (MahjongPai.Width+haiOffset), 0f, 0f );
                Hai hai = allUraDora[i];

                MahjongPai pai = PlayerUI.CreateMahjongPai(uraDoraRoot, pos, hai, false);
                _uraDoraHais.Add( pai );
            }
        }
    }

    void ClearMahjongList( List<MahjongPai> list )
    {
        if( list == null )
            return;

        for( int i = 0; i < list.Count; i++ )
        {
            PlayerUI.CollectMahjongPai(list[i]);
        }
        list.Clear();
    }

    void ClearYakuItemList( List<UIYakuItem> list )
    {
        if( list == null )
            return;

        for( int i = 0; i < list.Count; i++ )
        {
            Destroy( list[i] );
        }
        list.Clear();
    }


    public void ShowOmoteDora(int count)
    {
        for( int i = 0; i < _omoteDoraHais.Count; i++ )
        {
            if( i < count )
                _omoteDoraHais[i].Show();
            else
                _omoteDoraHais[i].Hide();
        }
    }

    public void ShowUraDora(int count)
    {
        for( int i = 0; i < _uraDoraHais.Count; i++ )
        {
            if( i < count )
                _uraDoraHais[i].Show();
            else
                _uraDoraHais[i].Hide();
        }
            
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        InitDoraHais();
        InitYakuInfo();

        gameObject.SetActive(true);

        MahjongMain logic = GameManager.Instance.MahjongMain;

        Hai addHai = logic.isTsumo? logic.TsumoHai : logic.SuteHai;

        int doraCount = logic.getOmotoDoras().Length;
        ShowOmoteDora( doraCount );

        if( logic.isTsumo == true )
            ShowUraDora( doraCount );


        Player player = GameManager.Instance.MahjongMain.ActivePlayer;
        tehai.BindPlayer(player);
        fuuro.BindPlayer(player);

        Hai[] hais = player.Tehai.getJyunTehai();
        tehai.SetTehai( hais, false );

        tehai.AddPai( addHai, true, true );
        tehai.SetAllHaisVisiable( true );

        Fuuro[] fuuros = player.Tehai.getFuuros();
        fuuro.UpdateFuuro( fuuros );

        StartCoroutine( ShowYakuOneByOne( logic.AgariInfo ) );
    }

    IEnumerator ShowYakuOneByOne( AgariInfo agariInfo )
    {
        yield return new WaitForSeconds(0.5f);

        var yakuArr = agariInfo.hanteiYakus;

        for( int i = 0; i < yakuArr.Length; i++ )
        {
            var yaku = yakuArr[i];

            string yakuName = yaku.getYakuNameKey();

            UIYakuItem item;

            if( yaku.isYakuman() ){
                item = CreateYakuItem_Yakuman( yakuName, yaku.isDoubleYakuman() );
            }
            else{
                item = CreateYakuItem( yakuName, yaku.getHanSuu() );
            }

            item.transform.parent = yakuRoot;
            item.transform.localScale = yakuItemPrefab.transform.localScale;
            item.transform.localPosition = new Vector3( yakuItemPosOffset.x, yakuItemPosOffset.y * (i+1), 0f );

            yield return new WaitForSeconds( yakuDisplayTime );
        }

        ShowTotalScrote( agariInfo );
    }

    void ShowTotalScrote( AgariInfo agariInfo )
    {
        int yakumanCount = 0;
        int totalHan = 0;

        var yakuArr = agariInfo.hanteiYakus;

        for( int i = 0; i < yakuArr.Length; i++ )
        {
            var yaku = yakuArr[i];

            if( yaku.isDoubleYakuman() ){
                yakumanCount += 2;
            }
            else if( yaku.isYakuman() ){
                yakumanCount += 1;
            }
            else{
                totalHan += yaku.getHanSuu();
            }
        }

        MahjongMain logic = GameManager.Instance.MahjongMain;
        int index = logic.getPlayerIndex( logic.ActivePlayer.JiKaze );
        bool isOya = index == logic.OyaIndex;

        int point = 0;

        if( yakumanCount > 0 ){
            SetYakuman();

            point = yakumanCount * (isOya? 48000 : 32000);
            SetPoint( point );
        }
        else{
            int level = 0;

            if( totalHan < 5 ){
                level = 0;
                point = 2000;
            }
            else if( totalHan < 6 ){ //5     满贯.
                level = 1;
                point = isOya? 12000: 8000;
            }
            else if( totalHan < 8 ){ //6-7   跳满
                level = 2;
                point = isOya? 18000: 12000;
            }
            else if( totalHan < 11 ){ //9-10 倍满.
                level = 3;
                point = isOya? 24000: 18000;
            }
            else if( totalHan < 13 ){ //11-12 三倍满.
                level = 4;
                point = isOya? 36000: 24000;
            }
            else{                     //13 役满.
                level = 5;
                point = isOya? 48000: 32000;
            }

            SetHan( totalHan, 20, level );
            SetPoint( point );
        }
    }

    void SetHan( int han, int fu, int level )
    {
        string levelName = ResManager.getString( GetYakuLevelNameKey(level) );

        lab_han.color = Color.white;
        lab_han.text = string.Format("[{0}]{1}{2}  {3}{4}[-]  [{5}]{6}[-]", 
                                     YakuHanColorStr, fu, ResManager.getString("fu"),
                                     han, ResManager.getString("han"),
                                     YakuLevelColorStr, levelName);
    }

    void SetYakuman()
    {
        lab_han.color = Color.white;
        lab_han.text = string.Format("[{0}]{1}[-]", YakuLevelColorStr, ResManager.getString("yakuman"));
    }

    void SetPoint( int point )
    {
        lab_point.text = point.ToString() + ResManager.getString("ten");
    }

    string GetYakuLevelNameKey(int level)
    {
        switch( level )
        {
            default:
            case 0: return "";

            case 1: return "mangan";
            case 2: return "haneman";
            case 3: return "baiman";
            case 4: return "sanbaiman";
            case 5: return "yakuman";
        }
    }


    UIYakuItem CreateYakuItem( string yakuNameKey, int han )
    {
        GameObject item = Instantiate( yakuItemPrefab ) as GameObject;
        item.SetActive( true );
        UIYakuItem comp = item.GetComponent<UIYakuItem>();
        comp.SetYaku( yakuNameKey, han );

        return comp;
    }
    UIYakuItem CreateYakuItem_Yakuman( string yakuNameKey, bool doubleYakuman )
    {
        GameObject item = Instantiate( yakuItemPrefab ) as GameObject;
        item.SetActive( true );
        UIYakuItem comp = item.GetComponent<UIYakuItem>();
        comp.SetYakuMan( yakuNameKey, doubleYakuman );

        return comp;
    }

}

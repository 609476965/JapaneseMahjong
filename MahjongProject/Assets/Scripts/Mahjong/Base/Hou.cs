﻿
/// <summary>
/// 河を管理する。
/// 日本麻将，玩家打出去的牌放置的那块区域叫河。
/// </summary>

public class Hou 
{
    // 捨牌の配列の長さの最大値.
    public readonly static int SUTE_HAIS_LENGTH_MAX = 24;

    // 捨牌の配列.
    private SuteHai[] _suteHais = new SuteHai[SUTE_HAIS_LENGTH_MAX];

    // 捨牌の配列の長さ.
    private int _suteHaisLength = 0;


    // 河を作成する.
    public Hou()
    {
        initialize();

        for (int i = 0; i < SUTE_HAIS_LENGTH_MAX; i++) {
            _suteHais[i] = new SuteHai();
        }
    }

    // 河を初期化する
    public void initialize()
    {
        _suteHaisLength = 0;
    }

    // 河をコピーする
    public static void copy(Hou dest, Hou src) 
    {
        dest._suteHaisLength = src._suteHaisLength;

        for (int i = 0; i < dest._suteHaisLength; i++) {
            SuteHai.copy(dest._suteHais[i], src._suteHais[i]);
        }
    }


    // 捨牌の配列を取得する
    public SuteHai[] getSuteHais()
    {
        return _suteHais;
    }
        
    // 捨牌の配列の長さを取得する
    public int getSuteHaisLength()
    {
        return _suteHaisLength;
    }

    // 捨牌の配列に牌を追加する
    public bool addHai(Hai hai)
    {
        if (_suteHaisLength >= SUTE_HAIS_LENGTH_MAX) {
            return false;
        }

        SuteHai.copy(_suteHais[_suteHaisLength], hai);
        _suteHaisLength++;

        return true;
    }

    // 捨牌の配列の最後の牌に、鳴きフラグを設定する
    public bool setNaki(bool isNaki)
    {
        if (_suteHaisLength <= 0) {
            return false;
        }

        _suteHais[_suteHaisLength - 1].IsNaki = isNaki;

        return true;
    }

    // 捨牌の配列の最後の牌に、リーチフラグを設定する
    public bool setReach(bool isReach)
    {
        if (_suteHaisLength <= 0) {
            return false;
        }

        _suteHais[_suteHaisLength - 1].IsReach = isReach;

        return true;
    }

    // 捨牌の配列の最後の牌に、手出しフラグを設定する
    public bool setTedashi(bool isTedashi)
    {
        if (_suteHaisLength <= 0) {
            return false;
        }

        _suteHais[_suteHaisLength - 1].IsTedashi = isTedashi;

        return true;
    }
}

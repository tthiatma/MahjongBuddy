﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MahjongBuddy.Models
{

    public enum WinningType
    {
        Straight,
        Pong,
        MixPureHand,
        PureHand,
        PureHonorHand,
        SevenPairs,
        LittleDragon,
        BigDragon,
        LittleFourWind,
        BigFourWind,
        AllKong,
        AllHiddenPongAndSelfPick,
        AllTerminal,
        //additional points
        SelfDraw,
        ConcealedHand,
        WinOnLastTile,
        OneGoodFlower,
        TwoGoodFlower,
        AllFourFlowerSameType,
        OneGoodWind,
        TwoGoodWind
    }
}
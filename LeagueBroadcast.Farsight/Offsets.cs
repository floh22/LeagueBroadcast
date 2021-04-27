using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Farsight
{
    class Offsets
    {
        static int GameTime = 0x2FD63C8;

        static int ObjIndex = 0x20;
        static int ObjTeam = 0x4C;
        static int ObjNetworkID = 0xCC;
        static int ObjPos = 0x1d8;
        static int ObjVisibility = 0x270;
        static int ObjSpawnCount = 0x284;
        static int ObjSrcIndex = 0x290;
        static int ObjMana = 0x298;
        static int ObjHealth = 0xD98;
        static int ObjMaxHealth = 0xDA8;
        static int ObjArmor = 0x12C4;
        static int ObjMagicRes = 0x12CC;
        static int ObjBaseAtk = 0x129C;
        static int ObjBonusAtk = 0x1218;
        static int ObjMoveSpeed = 0x12DC;
        static int ObjSpellBook = 0x2BA0;
        static int ObjName = 0x2F8C;
        static int ObjLvl = 0x36DC;
        static int ObjExpiry = 0x298;
        static int ObjCrit = 0x12C0;
        static int ObjCritMulti = 0x12B0;
        static int ObjAbilityPower = 0x1228;
        static int ObjAtkSpeedMulti = 0x1270;
        static int ObjItemList = 0x3714;

        static int ItemListItem = 0xC;
        static int ItemInfo = 0x20;
        static int ItemInfoId = 0x68;

        static int ViewProjMatrices = 0x3001530;
        static int Renderer = 0x300438C;
        static int RendererWidth = 0x0C;
        static int RendererHeight = 0x10;

        static int SpellSlotLevel = 0x20;
        static int SpellSlotTime = 0x28;
        static int SpellSlotDamage = 0x94;
        static int SpellSlotSpellInfo = 0x13C;
        static int SpellInfoSpellData = 0x44;
        static int SpellDataSpellName = 0x64;
        static int SpellDataMissileName = 0x64;

        static int ObjectManager = 0x1741890;
        static int LocalPlayer = 0x2FDE470;
        static int UnderMouseObject = 0x2FD2A48;

        static int ObjectMapCount = 0x2C;
        static int ObjectMapRoot = 0x28;
        static int ObjectMapNodeNetId = 0x10;
        static int ObjectMapNodeObject = 0x14;

        static int MissileSpellInfo = 0x258;
        static int MissileSrcIdx = 0x2B8;
        static int MissileDestIdx = 0x310;
        static int MissileStartPos = 0x2D0;
        static int MissileEndPos = 0x2DC;

        static int MinimapObject = 0x02FDE444;
        static int MinimapObjectHud = 0x88;
        static int MinimapHudPos = 0x60;
        static int MinimapHudSize = 0x68;
    }
}

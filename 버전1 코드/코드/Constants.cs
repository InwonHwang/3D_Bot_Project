using System;

internal static class Constants {
    internal enum PART : int { HEAD = 0, BODY, ARM_LEFT, ARM_RIGHT, LEG_LEFT, LEG_RIGHT, UPPER_BODY, UPPPERARM_LEFT, UPPERARM_RIGHT };
    internal enum ANIMSTATE : int { DEAD, IDLE, TURNLEFT, TURNRIGHT, WALK, JUMP };
    internal static readonly string[] stickerParent = {"Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head",
    "Bip001/Bip001 Pelvis",
    "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm",
    "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm",
    "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 L Thigh/Bip001 L Calf/Bip001 L Foot",
    "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 R Thigh/Bip001 R Calf/Bip001 R Foot",
    "Bip001/Bip001 Pelvis",
    "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm",
    "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm"};

    internal static readonly string[] imageNames = { "Go", "Left", "Right", "Jump", "G1", "G2", "Action" };
    internal const float Time = 1f;    

    internal static float state = 0;
}

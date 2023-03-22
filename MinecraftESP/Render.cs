﻿using ESP.Hood;
using ESP.Structs;
using ESP.Structs.Options;
using ESP.Utils;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Enums;
using Render = ESP.Render;
using RH = ESP.RenderHook;
using RU = ESP.Utils.RenderUtils;

namespace ESP;
public unsafe class Render
{
    public Settings Settings = new Settings();

    public bool Enable(ref Cap cap)
    {
        if (cap == Cap.Lighting && Settings.NoLight) { }
        else if (cap == Cap.Fog && Settings.NoFog) { }
        else if (cap == Cap.CullFace && Settings.AntiCullFace) { }
        else if (cap == Cap.DepthTest && Settings.CaveViewer) { }
        else return true;

        return false;
    }

    public bool Disable(ref Cap cap)
    {
        if (cap == Cap.Blend && Settings.WorldChams) { }
        else if (cap == Cap.Texture2D && Settings.NoBackground) { }
        else return true;

        return false;
    }

    public bool Begin(ref Mode mode)
    {
        if (mode == Mode.TrianglesStript && Settings.RainbowText)
        {
            (float r, float g, float b) color = RGB.GetF();
            GL.Color3f(color.r, color.g, color.b);
        }
        
        return true;
    }

    public bool TranslateF(ref float x, ref float y, ref float z)
    {
        if (x == .5 && y == .4375 && z == .9375)
            SetTarget(Settings.Chest, 0, .0625f, -.4375f);
        else if (x == 1 && y == 0.4375 && z == 0.9375)
            SetTarget(Settings.LargeChest, 0, .0625f, -.4375f);

        return true;
    }

    public bool ScaleF(ref float x, ref float y, ref float z)
    {
        if (x == .9375 && y == .9375 && z == .9375)
            SetTarget(Settings.Player, 0, -1, 0);
        else if (x == .25 && y == .25 && z == .25)
            SetTarget(Settings.Item);
        else if (x == .5 && y == .5 && z == .5)
            SetTarget(Settings.Item);
        else if (x.IsBetween(.666665f, .6666668f) && y.IsBetween(-.6666668f, -.666665f) && z.IsBetween(-.6666668f, -.666665f))
            SetTarget(Settings.Sign);
        else
        {
            Log.Write(x);
            Log.Write(" ");
            Log.Write(y);
            Log.Write(" ");
            Log.Write(z);
            Log.WriteLine(" ");
            SetTarget(Settings.Other);
        }

        return true;
    }

    public bool Ortho(ref double left, ref double right, ref double bottom, ref double top, ref double zNear, ref double zFar)
    {
        if (zNear != 1000 || zFar != 3000)
            return true;

        if (!Settings.ESP)
            return true;

        GL.PushAttrib(0x000fffff);
        GL.PushMatrix();

        RH.Disable(Cap.Texture2D);
        RH.Disable(Cap.CullFace);
        RH.Disable(Cap.Lighting);
        RH.Disable(Cap.DepthTest);

        RH.Enable(Cap.LineSmooth);

        RH.Enable(Cap.Blend);
        GL.BlendFunc(Factor.SrcAlpha, Factor.OneMinusSrcAlpha);

        foreach (TargetOpt targetOpt in Settings.AsList)
            if (targetOpt.Enabled)
                foreach (GLTarget target in targetOpt.Targets)
                {
                    if (!target.IsValid)
                        break;

                    target.DrawOver(targetOpt);
                }

        GL.PopAttrib();
        GL.PopMatrix();

        return true;
    }

    public void SwapBuffers(IntPtr hdc)
    {
        foreach (TargetOpt targetOpt in Settings.AsList)
        {
            for (int i = 0; i < targetOpt.Targets.Length; i++)
                targetOpt.Targets[i].IsValid = false;
            targetOpt.Index = 0;
        }
    }

    private int index;
    private float[] m3 = new float[4];
    private void SetTarget(TargetOpt options, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
    {
        try
        {
            index  = options.Index % 256;
            options.Targets[index].IsValid = true;
            GL.Interface.glGetFloatv(PName.ProjectionMatrix, options.Targets[index].Projection);
            GL.Interface.glGetFloatv(PName.ModelviewMatrix, options.Targets[index].Modelview);

            for (int i = 0; i < 4; ++i)
                m3[i] = options.Targets[index].Modelview[i] * offsetX + options.Targets[index].Modelview[i + 4] * offsetY + options.Targets[index].Modelview[i + 8] * offsetZ + options.Targets[index].Modelview[i + 12];

            options.Targets[index].Modelview[12] = m3[0];
            options.Targets[index].Modelview[13] = m3[1];
            options.Targets[index].Modelview[14] = m3[2];
            options.Targets[index].Modelview[15] = m3[3];

            options.Targets[index].DrawDuring(options);

            options.Index = index + 1;
        }
        catch (Exception ex) { Interop.MessageBox(0, ex.Message, "C#", 0); }
    }
}
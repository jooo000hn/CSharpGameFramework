﻿using System;
using System.Collections;
using System.Collections.Generic;
using StorySystem;
using ScriptRuntime;
using GameFramework;
using GameFramework.Skill;
using GameFrameworkMessage;

namespace GameFramework.Story.Commands
{
    /// <summary>
    /// createnpc(npc_unit_id,vector3(x,y,z),dir,camp,linkId[,ai,stringlist("param1 param2 param3 ..."),leaderId])[objid("@objid")];
    /// </summary>
    public class CreateNpcCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            CreateNpcCommand cmd = new CreateNpcCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Pos = m_Pos.Clone();
            cmd.m_Dir = m_Dir.Clone();
            cmd.m_Camp = m_Camp.Clone();
            cmd.m_LinkId = m_LinkId.Clone();
            cmd.m_AiLogic = m_AiLogic.Clone();
            cmd.m_AiParams = m_AiParams.Clone();
            cmd.m_LeaderId = m_LeaderId.Clone();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_HaveObjId = m_HaveObjId;
            cmd.m_ObjIdVarName = m_ObjIdVarName.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            if (m_ParamNum >= 5) {
                m_UnitId.Evaluate(instance, iterator, args);
                m_Pos.Evaluate(instance, iterator, args);
                m_Dir.Evaluate(instance, iterator, args);
                m_Camp.Evaluate(instance, iterator, args);
                m_LinkId.Evaluate(instance, iterator, args);

                if (m_ParamNum > 6) {
                    m_AiLogic.Evaluate(instance, iterator, args);
                    m_AiParams.Evaluate(instance, iterator, args);
                    if (m_ParamNum > 7) {
                        m_LeaderId.Evaluate(instance, iterator, args);
                    }
                }
            }
            if (m_HaveObjId) {
                m_ObjIdVarName.Evaluate(instance, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int objId = 0;
                if (m_ParamNum >= 5) {
                    Vector3 pos = m_Pos.Value;
                    float dir = m_Dir.Value;
                    int camp = m_Camp.Value;
                    int linkId = m_LinkId.Value;
                    
                    if (m_ParamNum > 6) {
                        string aiLogic = m_AiLogic.Value;
                        List<string> aiParams = new List<string>();
                        IEnumerable aiParamEnumer = m_AiParams.Value;
                        foreach (string aiParam in aiParamEnumer) {
                            aiParams.Add(aiParam);
                        }
                        objId = scene.CreateEntity(m_UnitId.Value, pos.X, pos.Y, pos.Z, dir, camp, linkId, aiLogic, aiParams.ToArray());
                    } else {
                        objId = scene.CreateEntity(m_UnitId.Value, pos.X, pos.Y, pos.Z, dir, camp, linkId);
                    }
                    if (m_ParamNum > 6) {
                        EntityInfo charObj = scene.GetEntityById(objId);
                        if (null != charObj) {
                            if (m_ParamNum > 7) {
                                int leaderId = m_LeaderId.Value;
                                charObj.GetAiStateInfo().LeaderID = leaderId;
                            } else {
                                charObj.GetAiStateInfo().LeaderID = 0;
                            }
                        }
                    }
                    EntityInfo obj = scene.GetEntityById(objId);
                    if (null != obj) {
                        Msg_RC_CreateNpc msg = DataSyncUtility.BuildCreateNpcMessage(obj);
                        scene.NotifyAllUser(RoomMessageDefine.Msg_RC_CreateNpc, msg);
                    }
                }
                if (m_HaveObjId) {
                    string varName = m_ObjIdVarName.Value;
                    instance.SetVariable(varName, objId);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum >= 5) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Pos.InitFromDsl(callData.GetParam(1));
                m_Dir.InitFromDsl(callData.GetParam(2));
                m_Camp.InitFromDsl(callData.GetParam(3));
                m_LinkId.InitFromDsl(callData.GetParam(4));

                if (m_ParamNum > 6) {
                    m_AiLogic.InitFromDsl(callData.GetParam(5));
                    m_AiParams.InitFromDsl(callData.GetParam(6));
                    if (m_ParamNum > 7) {
                        m_LeaderId.InitFromDsl(callData.GetParam(7));
                    }
                }
            }
        }

        protected override void Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First;
                Dsl.FunctionData second = statementData.Second;
                if (null != first && null != first.Call && null != second && null != second.Call) {
                    Load(first.Call);
                    LoadVarName(second.Call);
                }
            }
        }

        private void LoadVarName(Dsl.CallData callData)
        {
            if (callData.GetId() == "objid" && callData.GetParamNum() == 1) {
                m_ObjIdVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObjId = true;
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private int m_ParamNum = 0;
        private IStoryValue<Vector3> m_Pos = new StoryValue<Vector3>();
        private IStoryValue<float> m_Dir = new StoryValue<float>();
        private IStoryValue<int> m_Camp = new StoryValue<int>();
        private IStoryValue<int> m_LinkId = new StoryValue<int>();
        private IStoryValue<string> m_AiLogic = new StoryValue<string>();
        private IStoryValue<IEnumerable> m_AiParams = new StoryValue<IEnumerable>();
        private IStoryValue<int> m_LeaderId = new StoryValue<int>();
        private bool m_HaveObjId = false;
        private IStoryValue<string> m_ObjIdVarName = new StoryValue<string>();
    }
    /// <summary>
    /// destroynpc(npc_unit_id);
    /// </summary>
    public class DestroyNpcCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            DestroyNpcCommand cmd = new DestroyNpcCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                EntityInfo entity = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != entity) {
                    entity.NeedDelete = true;
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
    }
    /// <summary>
    /// destroynpcwithobjid(npc_obj_id);
    /// </summary>
    public class DestroyNpcWithObjIdCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            DestroyNpcWithObjIdCommand cmd = new DestroyNpcWithObjIdCommand();
            cmd.m_ObjId = m_ObjId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_ObjId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int objid = m_ObjId.Value;
                EntityInfo entity = scene.SceneContext.GetEntityById(objid);
                if (null != entity) {
                    entity.NeedDelete = true;
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }

        private IStoryValue<int> m_ObjId = new StoryValue<int>();
    }
    /// <summary>
    /// npcface(npc_unit_id,dir);
    /// </summary>
    public class NpcFaceCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcFaceCommand cmd = new NpcFaceCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Dir = m_Dir.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Dir.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                float dir = m_Dir.Value;
                EntityInfo entity = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != entity) {
                    MovementStateInfo msi = entity.GetMovementStateInfo();
                    msi.SetFaceDir(dir);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Dir.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<float> m_Dir = new StoryValue<float>();
    }
    /// <summary>
    /// npcmove(npc_unit_id,vector3(x,y,z));
    /// </summary>
    public class NpcMoveCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcMoveCommand cmd = new NpcMoveCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Pos = m_Pos.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Pos.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                Vector3 pos = m_Pos.Value;
                EntityInfo entity = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != entity) {
                    List<Vector3> waypoints = new List<Vector3>();
                    waypoints.Add(pos);
                    AiStateInfo aiInfo = entity.GetAiStateInfo();
                    AiData_ForMoveCommand data = aiInfo.AiDatas.GetData<AiData_ForMoveCommand>();
                    if (null == data) {
                        data = new AiData_ForMoveCommand(waypoints);
                        aiInfo.AiDatas.AddData(data);
                    }
                    data.WayPoints = waypoints;
                    data.Index = 0;
                    data.IsFinish = false;
                    entity.GetMovementStateInfo().TargetPosition = pos;
                    aiInfo.Time = 1000;//下一帧即触发移动
                    aiInfo.ChangeToState((int)PredefinedAiStateId.MoveCommand);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Pos.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<Vector3> m_Pos = new StoryValue<Vector3>();
    }
    /// <summary>
    /// npcmovewithwaypoints(npc_unit_id,vector3list("1 2 3 4 5 6"));
    /// </summary>
    public class NpcMoveWithWaypointsCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcMoveWithWaypointsCommand cmd = new NpcMoveWithWaypointsCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_WayPoints = m_WayPoints.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_WayPoints.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                List<object> poses = m_WayPoints.Value;
                EntityInfo entity = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != entity && null != poses && poses.Count > 0) {
                    List<Vector3> waypoints = new List<Vector3>();
                    waypoints.Add(entity.GetMovementStateInfo().GetPosition3D());
                    for (int i = 0; i < poses.Count; ++i) {
                        Vector3 pt = (Vector3)poses[i];
                        waypoints.Add(pt);
                    }
                    AiStateInfo aiInfo = entity.GetAiStateInfo();
                    AiData_ForMoveCommand data = aiInfo.AiDatas.GetData<AiData_ForMoveCommand>();
                    if (null == data) {
                        data = new AiData_ForMoveCommand(waypoints);
                        aiInfo.AiDatas.AddData(data);
                    }
                    data.WayPoints = waypoints;
                    data.Index = 0;
                    data.IsFinish = false;
                    entity.GetMovementStateInfo().TargetPosition = waypoints[0];
                    aiInfo.Time = 1000;//下一帧即触发移动
                    aiInfo.ChangeToState((int)PredefinedAiStateId.MoveCommand);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_WayPoints.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<List<object>> m_WayPoints = new StoryValue<List<object>>();
    }
    /// <summary>
    /// npcstop(npc_unit_id);
    /// </summary>
    public class NpcStopCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcStopCommand cmd = new NpcStopCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                EntityInfo entity = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != entity) {
                    AiStateInfo aiInfo = entity.GetAiStateInfo();
                    if (aiInfo.CurState == (int)PredefinedAiStateId.MoveCommand) {
                        aiInfo.Time = 0;
                        aiInfo.Target = 0;
                    }
                    entity.GetMovementStateInfo().IsMoving = false;
                    if (aiInfo.CurState > (int)PredefinedAiStateId.Invalid)
                        aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
    }
    /// <summary>
    /// npcattack(npc_unit_id[,target_unit_id]);
    /// </summary>
    public class NpcAttackCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcAttackCommand cmd = new NpcAttackCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_TargetUnitId = m_TargetUnitId.Clone();
            cmd.m_ParamNum = m_ParamNum;
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_TargetUnitId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                EntityInfo entity = scene.SceneContext.GetEntityByUnitId(unitId);
                EntityInfo target = null;
                int targetUnitId = m_TargetUnitId.Value;
                target = scene.SceneContext.GetEntityByUnitId(targetUnitId);
                if (null != entity && null != target) {
                    AiStateInfo aiInfo = entity.GetAiStateInfo();
                    aiInfo.Target = target.GetId();
                    aiInfo.LastChangeTargetTime = TimeUtility.GetLocalMilliseconds();
                    aiInfo.ChangeToState((int)PredefinedAiStateId.Idle);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_TargetUnitId.InitFromDsl(callData.GetParam(1));
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_TargetUnitId = new StoryValue<int>();
    }
    /// <summary>
    /// setformation(npc_unit_id,index);
    /// </summary>
    public class NpcSetFormationCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetFormationCommand cmd = new NpcSetFormationCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_FormationIndex = m_FormationIndex.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_FormationIndex.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(m_UnitId.Value);
                if (null != obj) {
                    obj.GetMovementStateInfo().FormationIndex = m_FormationIndex.Value;
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_FormationIndex.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_FormationIndex = new StoryValue<int>();
    }
    /// <summary>
    /// enableai(npc_unit_id,true_or_false);
    /// </summary>
    public class NpcEnableAiCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcEnableAiCommand cmd = new NpcEnableAiCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Enable = m_Enable.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Enable.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(m_UnitId.Value);
                if (null != obj) {
                    obj.SetAIEnable(m_Enable.Value != "false");
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Enable.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<string> m_Enable = new StoryValue<string>();
    }
    /// <summary>
    /// setai(unitid,ai_logic_id,stringlist("param1 param2 param3 ..."));
    /// </summary>
    public class NpcSetAiCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetAiCommand cmd = new NpcSetAiCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_AiLogic = m_AiLogic.Clone();
            cmd.m_AiParams = m_AiParams.Clone();
            return cmd;
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_AiLogic.Evaluate(instance, iterator, args);
            m_AiParams.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                string aiLogic = m_AiLogic.Value;
                IEnumerable aiParams = m_AiParams.Value;
                EntityInfo charObj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != charObj) {
                    charObj.GetAiStateInfo().Reset();
                    charObj.GetAiStateInfo().AiLogic = aiLogic;
                    int ix = 0;
                    foreach (string aiParam in aiParams) {
                        if (ix < AiStateInfo.c_MaxAiParamNum) {
                            charObj.GetAiStateInfo().AiParam[ix] = aiParam;
                            ++ix;
                        } else {
                            break;
                        }
                    }
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 2) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_AiLogic.InitFromDsl(callData.GetParam(1));
                m_AiParams.InitFromDsl(callData.GetParam(2));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<string> m_AiLogic = new StoryValue<string>();
        private IStoryValue<IEnumerable> m_AiParams = new StoryValue<IEnumerable>();
    }
    /// <summary>
    /// setaitarget(unitid,targetId);
    /// </summary>
    public class NpcSetAiTargetCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetAiTargetCommand cmd = new NpcSetAiTargetCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_TargetId = m_TargetId.Clone();
            return cmd;
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_TargetId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                int targetId = m_TargetId.Value;
                EntityInfo charObj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != charObj) {
                    charObj.GetAiStateInfo().Target = targetId;
                    charObj.GetAiStateInfo().HateTarget = targetId;
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_TargetId.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_TargetId = new StoryValue<int>();
    }
    /// <summary>
    /// npcanimation(unit_id, anim);
    /// </summary>
    public class NpcAnimationCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcAnimationCommand cmd = new NpcAnimationCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Anim = m_Anim.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Anim.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                string anim = m_Anim.Value;
                EntityInfo npcInfo = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != npcInfo) {
                    int objId = npcInfo.GetId();
                    EntityInfo obj = scene.EntityController.GetGameObject(objId);
                    if (null != obj) {
                        Msg_RC_PlayAnimation msg = new Msg_RC_PlayAnimation();
                        msg.obj_id = objId;
                        msg.anim_name = anim;
                        scene.NotifyAllUser(RoomMessageDefine.Msg_RC_PlayAnimation, msg);
                    }
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Anim.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<string> m_Anim = new StoryValue<string>();
    }
    /// <summary>
    /// npcaddimpact(unit_id, impactid, arg1, arg2, ...)[seq("@seq")];
    /// </summary>
    public class NpcAddImpactCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcAddImpactCommand cmd = new NpcAddImpactCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_ImpactId = m_ImpactId.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue<object> val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            cmd.m_HaveSeq = m_HaveSeq;
            cmd.m_SeqVarName = m_SeqVarName.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_ImpactId.Evaluate(instance, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue<object> val = m_Args[i];
                val.Evaluate(instance, iterator, args);
            }
            if (m_HaveSeq) {
                m_SeqVarName.Evaluate(instance, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                int impactId = m_ImpactId.Value;
                int seq = 0;
                Dictionary<string, object> locals = new Dictionary<string, object>();
                for (int i = 0; i < m_Args.Count - 1; i += 2) {
                    string key = m_Args[i].Value as string;
                    object val = m_Args[i + 1].Value;
                    if (!string.IsNullOrEmpty(key)) {
                        locals.Add(key, val);
                    }
                }
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != obj) {
                    ImpactInfo impactInfo = new ImpactInfo(impactId);
                    impactInfo.StartTime = TimeUtility.GetLocalMilliseconds();
                    impactInfo.ImpactSenderId = obj.GetId();
                    impactInfo.SkillId = 0;
                    if (null != impactInfo.ConfigData) {
                        obj.GetSkillStateInfo().AddImpact(impactInfo);
                        seq = impactInfo.Seq;
                        if (scene.SkillSystem.StartSkill(obj.GetId(), impactInfo.ConfigData, seq, locals)) {
                            Msg_RC_AddImpact addImpactBuilder = new Msg_RC_AddImpact();
                            addImpactBuilder.sender_id = obj.GetId();
                            addImpactBuilder.target_id = obj.GetId();
                            addImpactBuilder.impact_id = impactId;
                            addImpactBuilder.skill_id = -1;
                            addImpactBuilder.duration = impactInfo.DurationTime;
                            scene.NotifyAllUser(RoomMessageDefine.Msg_RC_AddImpact, addImpactBuilder);
                        }
                    }
                }
                if (m_HaveSeq) {
                    string varName = m_SeqVarName.Value;
                    instance.SetVariable(varName, seq);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_ImpactId.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
        }

        protected override void Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First;
                Dsl.FunctionData second = statementData.Second;
                if (null != first && null != first.Call && null != second && null != second.Call) {
                    Load(first.Call);
                    LoadVarName(second.Call);
                }
            }
        }

        private void LoadVarName(Dsl.CallData callData)
        {
            if (callData.GetId() == "seq" && callData.GetParamNum() == 1) {
                m_SeqVarName.InitFromDsl(callData.GetParam(0));
                m_HaveSeq = true;
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_ImpactId = new StoryValue<int>();
        private List<IStoryValue<object>> m_Args = new List<IStoryValue<object>>();
        private bool m_HaveSeq = false;
        private IStoryValue<string> m_SeqVarName = new StoryValue<string>();
    }
    /// <summary>
    /// npcremoveimpact(unit_id, seq);
    /// </summary>
    public class NpcRemoveImpactCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcRemoveImpactCommand cmd = new NpcRemoveImpactCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Seq = m_Seq.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Seq.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                int seq = m_Seq.Value;
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != obj) {
                    ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoBySeq(seq);
                    if (null != impactInfo) {
                        Msg_RC_RemoveImpact removeImpactBuilder = new Msg_RC_RemoveImpact();
                        removeImpactBuilder.obj_id = obj.GetId();
                        removeImpactBuilder.impact_id = impactInfo.ImpactId;
                        scene.NotifyAllUser(RoomMessageDefine.Msg_RC_RemoveImpact, removeImpactBuilder);

                        scene.SkillSystem.StopSkill(obj.GetId(), impactInfo.ImpactId, seq, true);
                        obj.GetSkillStateInfo().RemoveImpact(seq);
                    }
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Seq.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_Seq = new StoryValue<int>();
    }
    /// <summary>
    /// npccastskill(unit_id, skillid, arg1, arg2, ...);
    /// </summary>
    public class NpcCastSkillCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcCastSkillCommand cmd = new NpcCastSkillCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_SkillId = m_SkillId.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue<object> val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_SkillId.Evaluate(instance, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue<object> val = m_Args[i];
                val.Evaluate(instance, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                int skillId = m_SkillId.Value;
                Dictionary<string, object> locals = new Dictionary<string, object>();
                for (int i = 0; i < m_Args.Count - 1; i += 2) {
                    string key = m_Args[i].Value as string;
                    object val = m_Args[i + 1].Value;
                    if (!string.IsNullOrEmpty(key)) {
                        locals.Add(key, val);
                    }
                }
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != obj) {
                    SkillInfo skillInfo = obj.GetSkillStateInfo().GetSkillInfoById(skillId);
                    if (null != skillInfo) {
                        if (scene.SkillSystem.StartSkill(obj.GetId(), skillInfo.ConfigData, 0, locals)) {
                            Msg_RC_NpcSkill skillBuilder = new Msg_RC_NpcSkill();
                            skillBuilder.npc_id = obj.GetId();
                            skillBuilder.skill_id = skillId;
                            float x = obj.GetMovementStateInfo().GetPosition3D().X;
                            float z = obj.GetMovementStateInfo().GetPosition3D().Z;
                            skillBuilder.stand_pos = ProtoHelper.EncodePosition2D(x, z);
                            skillBuilder.face_direction = ProtoHelper.EncodeFloat(obj.GetMovementStateInfo().GetFaceDir());

                            scene.NotifyAllUser(RoomMessageDefine.Msg_RC_NpcSkill, skillBuilder);
                        }
                    }
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_SkillId.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_SkillId = new StoryValue<int>();
        private List<IStoryValue<object>> m_Args = new List<IStoryValue<object>>();
    }
    /// <summary>
    /// npcstopskill(unit_id);
    /// </summary>
    public class NpcStopSkillCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcStopSkillCommand cmd = new NpcStopSkillCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != obj) {
                    scene.SkillSystem.StopAllSkill(obj.GetId(), true);

                    Msg_RC_NpcStopSkill skillBuilder = new Msg_RC_NpcStopSkill();
                    skillBuilder.npc_id = obj.GetId();
                    scene.NotifyAllUser(RoomMessageDefine.Msg_RC_NpcStopSkill, skillBuilder);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_SkillId = new StoryValue<int>();
    }
    /// <summary>
    /// npcaddskill(unit_id, skillid);
    /// </summary>
    public class NpcAddSkillCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcAddSkillCommand cmd = new NpcAddSkillCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_SkillId = m_SkillId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_SkillId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                int skillId = m_SkillId.Value;
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != obj) {
                    if (obj.GetSkillStateInfo().GetSkillInfoById(skillId) == null) {
                        obj.GetSkillStateInfo().AddSkill(new SkillInfo(skillId));

                        Msg_RC_AddSkill msg = new Msg_RC_AddSkill();
                        msg.obj_id = obj.GetId();
                        msg.skill_id = skillId;
                        scene.NotifyAllUser(RoomMessageDefine.Msg_RC_AddSkill, msg);
                    }
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_SkillId.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_SkillId = new StoryValue<int>();
    }
    /// <summary>
    /// npcremoveskill(unit_id, skillid);
    /// </summary>
    public class NpcRemoveSkillCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcRemoveSkillCommand cmd = new NpcRemoveSkillCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_SkillId = m_SkillId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_SkillId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                int skillId = m_SkillId.Value;
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != obj) {
                    obj.GetSkillStateInfo().RemoveSkill(skillId);
                    
                    Msg_RC_RemoveSkill msg = new Msg_RC_RemoveSkill();
                    msg.obj_id = obj.GetId();
                    msg.skill_id = skillId;
                    scene.NotifyAllUser(RoomMessageDefine.Msg_RC_RemoveSkill, msg);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_SkillId.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_SkillId = new StoryValue<int>();
    }
    /// <summary>
    /// npclisten(unit_id, 消息类别, true_or_false);
    /// </summary>
    public class NpcListenCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcListenCommand cmd = new NpcListenCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_Event = m_Event.Clone();
            cmd.m_Enable = m_Enable.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_Event.Evaluate(instance, iterator, args);
            m_Enable.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                string eventName = m_Event.Value;
                string enable = m_Enable.Value;
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != obj) {
                    if (StoryListenFlagEnum.Damage == StoryListenFlagUtility.FromString(eventName)) {
                        if (0 == string.Compare(enable, "true"))
                            obj.AddStoryFlag(StoryListenFlagEnum.Damage);
                        else
                            obj.RemoveStoryFlag(StoryListenFlagEnum.Damage);
                    }
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 2) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_Event.InitFromDsl(callData.GetParam(1));
                m_Enable.InitFromDsl(callData.GetParam(2));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<string> m_Event = new StoryValue<string>();
        private IStoryValue<string> m_Enable = new StoryValue<string>();
    }
    /// <summary>
    /// setcamp(npc_unit_id,camp_id);
    /// </summary>
    public class NpcSetCampCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetCampCommand cmd = new NpcSetCampCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_CampId = m_CampId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_CampId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                EntityInfo obj = scene.SceneContext.GetEntityByUnitId(m_UnitId.Value);
                if (null != obj) {
                    int campId = m_CampId.Value;
                    obj.SetCampId(campId);

                    Msg_RC_CampChanged msg = new Msg_RC_CampChanged();
                    msg.obj_id = obj.GetId();
                    msg.camp_id = campId;
                    scene.NotifyAllUser(RoomMessageDefine.Msg_RC_CampChanged, msg);
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_CampId.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_CampId = new StoryValue<int>();
    }
    /// setsummonerid(unit_id, objid);
    /// </summary>
    public class NpcSetSummonerIdCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetSummonerIdCommand cmd = new NpcSetSummonerIdCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_SummonerId = m_SummonerId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_SummonerId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                int summonerId = m_SummonerId.Value;
                EntityInfo npcInfo = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != npcInfo) {
                    npcInfo.SummonerId = summonerId;
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_SummonerId.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_SummonerId = new StoryValue<int>();
    }
    /// setsummonskillid(unit_id, objid);
    /// </summary>
    public class NpcSetSummonSkillIdCommand : AbstractStoryCommand
    {
        public override IStoryCommand Clone()
        {
            NpcSetSummonSkillIdCommand cmd = new NpcSetSummonSkillIdCommand();
            cmd.m_UnitId = m_UnitId.Clone();
            cmd.m_SummonSkillId = m_SummonSkillId.Clone();
            return cmd;
        }

        protected override void ResetState()
        {
        }

        protected override void Evaluate(StoryInstance instance, object iterator, object[] args)
        {
            m_UnitId.Evaluate(instance, iterator, args);
            m_SummonSkillId.Evaluate(instance, iterator, args);
        }

        protected override bool ExecCommand(StoryInstance instance, long delta)
        {
            Scene scene = instance.Context as Scene;
            if (null != scene) {
                int unitId = m_UnitId.Value;
                int summonSkillId = m_SummonSkillId.Value;
                EntityInfo npcInfo = scene.SceneContext.GetEntityByUnitId(unitId);
                if (null != npcInfo) {
                    npcInfo.SummonSkillId = summonSkillId;
                }
            }
            return false;
        }

        protected override void Load(Dsl.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_UnitId.InitFromDsl(callData.GetParam(0));
                m_SummonSkillId.InitFromDsl(callData.GetParam(1));
            }
        }

        private IStoryValue<int> m_UnitId = new StoryValue<int>();
        private IStoryValue<int> m_SummonSkillId = new StoryValue<int>();
    }
}

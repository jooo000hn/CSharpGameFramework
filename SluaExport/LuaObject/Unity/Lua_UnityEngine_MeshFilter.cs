﻿using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_MeshFilter : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.MeshFilter o;
			o=new UnityEngine.MeshFilter();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mesh(IntPtr l) {
		try {
			UnityEngine.MeshFilter self=(UnityEngine.MeshFilter)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mesh);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mesh(IntPtr l) {
		try {
			UnityEngine.MeshFilter self=(UnityEngine.MeshFilter)checkSelf(l);
			UnityEngine.Mesh v;
			checkType(l,2,out v);
			self.mesh=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sharedMesh(IntPtr l) {
		try {
			UnityEngine.MeshFilter self=(UnityEngine.MeshFilter)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.sharedMesh);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sharedMesh(IntPtr l) {
		try {
			UnityEngine.MeshFilter self=(UnityEngine.MeshFilter)checkSelf(l);
			UnityEngine.Mesh v;
			checkType(l,2,out v);
			self.sharedMesh=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.MeshFilter");
		addMember(l,"mesh",get_mesh,set_mesh,true);
		addMember(l,"sharedMesh",get_sharedMesh,set_sharedMesh,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.MeshFilter),typeof(UnityEngine.Component));
	}
}

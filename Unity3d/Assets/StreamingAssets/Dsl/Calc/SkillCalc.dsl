//反击技能计算
proc(haogui_react)
{
	if(attr(2006)*5<attr(2005)){
		if(skill(8)>0){
			command(["story","objweak"],self());
		};
		const(1);
	}else{
		const(0);
	};
};
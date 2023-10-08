onGrund = false     --是否在地面上
-- public codeInput = "" -- 公共变量用于存储玩家输入的代码

function Awake()

end

function Start()

end

function Update()

    v = self:GetVelocity()      -- 获取角色当前的速度
    input = self:GetInput()     -- 获取玩家的输入
    
    if Input.GetKeyDown(KeyCode.Space) and onGrund then 
        onGrund = false
        v.y = 12                        -- 将角色的垂直速度设置为12，跳跃
    end

    self:SetVelocity(input.x * 5, v.y)  -- 根据玩家输入设置角色的水平速度

    if Input.GetKeyDown(KeyCode.LeftShift) then
        -- 冲刺（加速跑）/闪现（瞬移或短暂快速冲刺，不允许卡墙）/弹射（根据鼠标方位起跳，下落前玩家无法控制方向）
    end
end

function FixedUpdate()

end

function OnDestroy()
    
end

function OnHitGround()
    onGrund = true
end



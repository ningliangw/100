onGrund = false     --是否在地面上

function Awake()

end

function Start()

end

function Update()

    v = self:GetVelocity()      -- 获取角色当前的速度
    input = self:GetInput()     -- 获取玩家的输入

    
    if Input.GetKeyDown(KeyCode.Space) and onGrund then 
        -- 如果玩家按下空格键并且角色在地面上
        onGrund = false
        v.y = 12                -- 将角色的垂直速度设置为12，跳跃
    end

    self:SetVelocity(input.x * 5, v.y)  -- 根据玩家输入设置角色的水平速度
end

function FixedUpdate()

end

function OnDestroy()
    
end

function OnHitGround()
    onGrund = true
end
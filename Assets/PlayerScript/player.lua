onGrund = false

function Awake()

end

function Start()

end

function Update()

    v = self:GetVelocity()
    input = self:GetInput()

    if Input.GetKeyDown(KeyCode.Space) and onGrund then
        onGrund = false
        v.y = 12
    end

    self:SetVelocity(input.x * 5, v.y)
end

function FixedUpdate()

end

function OnDestroy()
    
end

function OnHitGround()
    onGrund = true
end
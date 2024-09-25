using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : MonoBehaviour
{
    // 移动速度
    [Header("推力，会给被推的物体施加一个额外速度")] public float moveSpeed = 5f;
    // 减速系数
    [Header("其它物体被推之后，恢复原速度的倍率，越大恢复的越快")] public float deceleration = 0.5f;

    

    // 记录与触发器碰撞的物体及其初始速度
    private Dictionary<Rigidbody2D, Vector2> collidedObjects = new Dictionary<Rigidbody2D, Vector2>();

    void OnTriggerEnter2D(Collider2D other)
    {
        DoDamage(other);

        // 检测到进入触发器的物体
        // 如果需要对特定类型的物体进行操作，可以添加条件判断
        // 例如：if (other.CompareTag("Player")) { ... }

        // 计算移动方向
        Vector2 moveDirection = (other.transform.position - transform.position).normalized;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (!collidedObjects.ContainsKey(rb)&&rb)
        {
            collidedObjects.Add(rb, rb.velocity);
        }
        // 将物体应用力向外
        if(rb)
            rb.AddForce(moveDirection * moveSpeed, ForceMode2D.Impulse);

        // 记录初始速度
    }

    protected virtual void DoDamage(Collider2D collider)
    {

    }

    void Update()
    {       
        // 循环检测碰撞物体字典中的物体
        foreach (KeyValuePair<Rigidbody2D, Vector2> pair in collidedObjects)
        {
            Rigidbody2D rb = pair.Key;
            Vector2 initialVelocity = pair.Value;

            if (rb == null)
            {
                collidedObjects.Remove(rb);
                continue;
            }

            // 计算与原始速度的差值
            Vector2 velocityDifference = initialVelocity - rb.velocity;
            // 如果差值大于0.01，进行减速
            if (velocityDifference.magnitude > 0.01f)
            {
                // 计算减去的速度
                Vector2 decelerationVector = velocityDifference.normalized * (deceleration * Time.deltaTime);
                // 减去速度
                rb.velocity += decelerationVector;
            }
            else
            {
                // 确保速度最终等于原始速度
                rb.velocity = initialVelocity;
                // 移除碰撞物体字典中的物体
                collidedObjects.Remove(rb);
                break; // 避免在循环中修改字典结构
            }
        }
    }

}

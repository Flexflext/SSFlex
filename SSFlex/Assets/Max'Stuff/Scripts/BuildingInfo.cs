using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    public enum EClipSlots
    {
        up,
        right,
        down,
        left,
        none
    }

    public List<EClipSlots> OccupiedSlots => mOccupiedSlots;
    private List<EClipSlots> mOccupiedSlots;

    [SerializeField]
    private float mHealth;

    [SerializeField]
    private MeshRenderer mMeshRenderer;

    [SerializeField]
    private float mColourDamageValue;

    [SerializeField]
    private ParticleSystem mDamagedParticle;

    private Color mDamagedColour;
    private float mRFloat;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(2);
        }
    }

    private void Start()
    {
        mOccupiedSlots = new List<EClipSlots>();
    }

    public void AddClipSlot(EClipSlots _slotToAdd)
    {
        mOccupiedSlots.Add(_slotToAdd);
    }

    public void RemoveClipSlot(EClipSlots _slotToRemove)
    {
        mOccupiedSlots.Remove(_slotToRemove);
    }

    public void TakeDamage(float _damage)
    {
        mHealth -= _damage;

        mRFloat += mColourDamageValue;
        mDamagedColour = new Color(mRFloat, 0,0,50);
        mDamagedColour = Color.HSVToRGB(0, 0.5f + mRFloat,1);
        mDamagedParticle.Play();

        mMeshRenderer.material.color = mDamagedColour;
    }
}

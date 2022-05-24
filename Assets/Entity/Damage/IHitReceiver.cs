namespace Entity.Damage
{
    /// <summary>
    /// Blueprint for objects that do things when hit
    /// E.g.
    /// - Update health
    /// - Bounce a laser
    /// - Activate a button
    /// - etc
    /// </summary>
    public interface IHitReceiver
    {
        /// <summary>
        /// Receive a hit, run some code
        /// </summary>
        /// <param name="data">all the data associated with the hit</param>
        public void ReceiveHit(HitData data);
    }
}
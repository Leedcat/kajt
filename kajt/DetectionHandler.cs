namespace kajt
{
    public delegate void ProjectileDelegate(long timeSinceProjectile, long timeSpentMoving);

    // TODO: Come up with a better name than `DetectionHandler`
    // TODO: Make DetectionHandler use another thread for loop
    class DetectionHandler
    {
        public event ProjectileDelegate? OnProjectile;

        public bool running
        {
            get;
            private set;
        }

        public DetectionHandler()
        {
            this.running = false;
        }

        private void loop()
        {
            long lastTime = DateTime.Now.Millisecond;
            long timeSinceProjectile = 0;
            long timeSpentMoving = 0;

            bool firstProjectile = true;

            while (this.running)
            {
                // Calculate time between frames
                long now = DateTime.Now.Millisecond;
                long deltaTime = now - lastTime;
                lastTime = now;

                // TOOD: Grab screenshot and process it
                bool projectile = false;
                bool moving = false;

                // Skip the first projectile
                if (firstProjectile && projectile)
                {
                    timeSinceProjectile = 0;
                    timeSpentMoving = 0;
                    firstProjectile = false;
                    continue;
                }
                else if (firstProjectile)
                {
                    continue;
                }

                // Add time to time variables
                timeSinceProjectile += deltaTime;
                if (moving) timeSpentMoving += deltaTime;

                // Trigger projectile event
                if (projectile)
                {
                    this.TriggerProjectile(timeSinceProjectile, timeSpentMoving);
                    timeSinceProjectile = 0;
                    timeSpentMoving = 0;
                }
            }
        }

        public void start()
        {
            this.running = true;
            this.loop();
        }

        public void stop()
        {
            this.running = false;
        }

        protected virtual void TriggerProjectile(long timeSinceProjectile, long timeSpentMoving)
        {
            this.OnProjectile?.Invoke(timeSinceProjectile, timeSpentMoving);
        }
    }
}